using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DTO.PollDTOs;
using Ankietyzator.Models.DTO.QuestionDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class PollingService : IPollingService
    {
        private const string BaseUrl = "https://cc-2020-group-one-ankietyzator-function.azurewebsites.net/api/";
        private const string Update = "UpdatePollStats?code=Et1tL0adY4uEKmeS3pK5/I9WzGf0BznFAnVO3CdtvUNYY30K5fLYLA==";
        
        private const string AccountNotFoundStr = "Account not found. That should not happen";
        private const string NoPollFormStr = "Could not find form with the specified ID";
        private const string InvalidIndexStr = "Questions must have distinct indexes";
        private const string NoQuestionsStr = "Poll has no questions";
        
        private const string PollFormsSuccessStr = "Poll forms fetched successfully";
        private const string PollRemovedStr = "Poll form removed successfully";
        private const string PollUpdatedStr = "Poll updated successfully";
        private const string PollCreatedStr = "Poll created successfully";
        private const string PrevPollNotFoundStr = "Could not find previous poll";
        private const string AccountMismatchStr = "You cannot modify this poll";

        private readonly AnkietyzatorDbContext _context;
        private readonly IQuestionService _questionService;
        private readonly IStatService _statService;
        private readonly IMapper _mapper;

        public PollingService(AnkietyzatorDbContext context, IMapper mapper, IQuestionService questionService,
            IStatService statService)
        {
            _questionService = questionService;
            _statService = statService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetPollFormDto>>> GetAllPollForms(bool archived)
        {
            var response = new ServiceResponse<List<GetPollFormDto>>();
            var dtoPolls = await _context.PollForms.Select(p => _mapper.Map<GetPollFormDto>(p)).ToListAsync();
            dtoPolls = dtoPolls.Where(p => archived ? p.Archived : !p.Archived).ToList();

            foreach (GetPollFormDto getPollFormDto in dtoPolls)
            {
                var subResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (subResponse.Data == null) return response.Failure(subResponse);
                await PrepareGetDto(getPollFormDto, subResponse.Data);
            }

            return response.Success(dtoPolls, PollFormsSuccessStr);
        }


        public async Task<ServiceResponse<List<GetPollFormDto>>> GetUserPollForms(string email, bool filled)
        {
            var response = new ServiceResponse<List<GetPollFormDto>>();
            var responseAll = await GetAllUserPollForms(email);
            if (responseAll.Data == null) return response.Failure(responseAll);
            var polls = responseAll.Data;

            var accountId = (await GetAccount(email)).Data.AccountId;

            var filledPolls = (
                from poll in polls
                join question in _context.Questions on poll.PollId equals question.Poll
                join answer in _context.Answers on question.QuestionId equals answer.QuestionId
                where answer.AccountId == accountId
                select poll
            ).Distinct().ToList();

            responseAll.Data = filled ? filledPolls : polls.Except(filledPolls).ToList();
            var dtoPolls = responseAll.Data.Select(p => _mapper.Map<GetPollFormDto>(p)).ToList();

            foreach (GetPollFormDto getPollFormDto in dtoPolls)
            {
                var questionsResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse);
                await PrepareGetDto(getPollFormDto, questionsResponse.Data);
            }

            return response.Success(dtoPolls, PollFormsSuccessStr);
        }

        private async Task<ServiceResponse<List<PollForm>>> GetAllUserPollForms(string email)
        {
            var response = new ServiceResponse<List<PollForm>>();
            var userResponse = await GetAccount(email);
            if (userResponse.Data == null) return response.Failure(userResponse);

            var user = userResponse.Data;
            string[] tags = user.Tags.Split('/');

            var polls = await _context.PollForms
                .Where(p => p.Emails.Contains(email) || tags[0] != "")
                .ToListAsync();
            var pollsTagged = new List<PollForm>();
            foreach (PollForm pollForm in polls)
            {
                string[] pollTags = pollForm.Tags.Split('/');
                if (pollTags.Intersect(tags).Any()) pollsTagged.Add(pollForm);
            }

            return response.Success(pollsTagged, PollFormsSuccessStr);
        }

        public async Task<ServiceResponse<List<GetPollFormDto>>> GetPollsterPollForms(string email, bool archived)
        {
            var response = await GetAllPollsterPollForms(email);
            if (response.Data == null) return response;

            var polls = response.Data.Where(p => archived ? p.Archived : !p.Archived).ToList();
            foreach (GetPollFormDto getPollFormDto in polls)
            {
                var questionsResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse);
                await PrepareGetDto(getPollFormDto, questionsResponse.Data);
            }

            response.Data = polls;
            return response;
        }

        private async Task<ServiceResponse<List<GetPollFormDto>>> GetAllPollsterPollForms(string email)
        {
            var response = new ServiceResponse<List<GetPollFormDto>>();
            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse);

            var pollster = pollsterResponse.Data;
            
            var pollForms = await _context.PollForms.Where(p => p.AuthorId == pollster.AccountId).ToListAsync();
            if (pollForms == null) return response.Success(null, PollFormsSuccessStr);
            

            var pollFormsDto = new List<GetPollFormDto>();
            foreach (PollForm pollForm in pollForms)
            {
                var questionsResponse = await GetQuestionsDto(pollForm, response, pollFormsDto);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse);
            }
            
            foreach (GetPollFormDto formDto in pollFormsDto)
            {
                formDto.AuthorEmail = pollster.EMail;
                formDto.AuthorName = pollster.Name;
            }

            return response.Success(pollFormsDto, PollFormsSuccessStr);
        }

        public async Task<ServiceResponse<GetPollFormDto>> ClosePollForm(int pollId, string email)
        {
            //TODO: refactor both methods
            var response = new ServiceResponse<GetPollFormDto>();

            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse);
            var account = pollsterResponse.Data;
            
            var pollForm = await _context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr, HttpStatusCode.NotFound);

            var condition = pollForm.AuthorId != account.AccountId && account.UserType != UserType.User;
            if (condition) return response.Failure(AccountMismatchStr, HttpStatusCode.Unauthorized);

            pollForm.Archived = true;
            await _context.SaveChangesAsync();
            return response.Success(_mapper.Map<GetPollFormDto>(pollForm), PollUpdatedStr);
        }

        public async Task<ServiceResponse<GetPollFormDto>> RemovePollForm(int pollId, string email)
        {
            var response = new ServiceResponse<GetPollFormDto>();

            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse);
            var account = pollsterResponse.Data;

            var pollForm = await _context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr, HttpStatusCode.NotFound);

            var condition = pollForm.AuthorId != account.AccountId && account.UserType == UserType.User;
            if (condition) return response.Failure(AccountMismatchStr, HttpStatusCode.Unauthorized);

            await _questionService.RemoveQuestions(pollId);
            _context.PollForms.Remove(pollForm);
            await _context.SaveChangesAsync();
            
            RunFunction(account.AccountId);
            
            return response.Success(_mapper.Map<GetPollFormDto>(pollForm), PollRemovedStr);
        }

        public async Task<ServiceResponse<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, string email)
        {
            var response = new ServiceResponse<GetPollFormDto>();
            var previousForm = await _context.PollForms.FindAsync(pollForm.PreviousPollId);
            if (previousForm == null) return response.Failure(PrevPollNotFoundStr, HttpStatusCode.NotFound);

            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse);
            var account = pollsterResponse.Data;
            bool fail = previousForm.AuthorId != account.AccountId && account.UserType != UserType.Admin;
            if (fail) return response.Failure(AccountMismatchStr, HttpStatusCode.Unauthorized);

            previousForm.Archived = true;
            var createPollFormDto = _mapper.Map<CreatePollFormDto>(pollForm);
            response = await CreatePollForm(createPollFormDto, email);

            if (response.Data == null) return response;
            response.Message = PollUpdatedStr;
            return response;
        }

        public async Task<ServiceResponse<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, string email)
        {
            var response = new ServiceResponse<GetPollFormDto>();
            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse);

            bool noQuestions = pollForm.Questions == null || pollForm.Questions.Count == 0;
            if (noQuestions) return response.Failure(NoQuestionsStr, HttpStatusCode.UnprocessableEntity);

            if (pollForm.Questions.Count != pollForm.Questions.Select(q => q.Position).Distinct().Count())
                return response.Failure(InvalidIndexStr, HttpStatusCode.UnprocessableEntity);

            var dalForm = _mapper.Map<PollForm>(pollForm);
            dalForm.AuthorId = pollsterResponse.Data.AccountId;
            await _context.PollForms.AddAsync(dalForm);
            await _context.SaveChangesAsync();

            //int topIndex = await _context.PollForms.CountAsync(); HAŃBA
            var questions = new List<GetQuestionDto>();
            foreach (CreateQuestionDto createQuestionDto in pollForm.Questions)
            {
                var questionResponse = await _questionService.CreateQuestion(createQuestionDto, dalForm.PollId);
                if (questionResponse.Data == null) return response.Failure(questionResponse);
                questions.Add(questionResponse.Data);
            }

            var pollResponse = await _statService.CreatePollStats(dalForm.PollId);
            if (pollResponse.Data == null) return response.Failure(pollResponse);

            var getPollFormDto = _mapper.Map<GetPollFormDto>(dalForm);
            getPollFormDto.Questions = questions;

            await _statService.CreateQuestionsStats(getPollFormDto.Questions);

            return response.Success(getPollFormDto, PollCreatedStr);
        }

        private async Task<ServiceResponse<T>> GetQuestionsDto<T>(
            PollForm updatePollFormDto,
            ServiceResponse<T> response,
            ICollection<GetPollFormDto> questions)
        {
            questions?.Clear();
            GetPollFormDto pollFormDto = _mapper.Map<GetPollFormDto>(updatePollFormDto);
            var questionsResponse = await _questionService.GetQuestions(updatePollFormDto.PollId);
            if (questionsResponse.Data == null) return response.Failure(questionsResponse);
            pollFormDto.Questions = questionsResponse.Data;
            questions?.Last().Questions.AddRange(questionsResponse.Data);
            return response.Success(default, questionsResponse.Message);
        }

        //======================= HELPER METHODS ===================//

        private async Task<ServiceResponse<Account>> GetAccount(string email)
        {
            var response = new ServiceResponse<Account>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            return account == null
                ? response.Failure(AccountNotFoundStr, HttpStatusCode.NotFound)
                : response.Success(account, "");
        }
        
        private async Task PrepareGetDto(GetPollFormDto getPollFormDto, List<GetQuestionDto> questions)
        {
            var author = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == getPollFormDto.AuthorId);
            getPollFormDto.Questions = questions;
            getPollFormDto.AuthorEmail = author.EMail;
            getPollFormDto.AuthorName = author.Name;
        }
        
        private static async void RunFunction(int authorId)
        {
            using var httpClient = new HttpClient();
            
            // Update poll stats
            var pollBuilder = new StringBuilder(BaseUrl).Append(Update);
            pollBuilder.Append($"&authorId={authorId}");
            var pollMessage = new HttpRequestMessage(HttpMethod.Get, pollBuilder.ToString());

            var pollResponse = await httpClient.SendAsync(pollMessage);
            dynamic pollResult = await pollResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Poll response: " + pollResult);
        }
    }
}
