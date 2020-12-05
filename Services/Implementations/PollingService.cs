using System.Collections.Generic;
using System.Linq;
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
        private const string AccountNotFoundStr = "Account not found. That should not happen";
        private const string NoPollFormStr = "Could not find form with the specified ID";
        private const string InvalidIndexStr = "Questions must have distinct indexes";

        private const string NoQuestionsStr = "Poll has no questions";

        //private const string PollFormSuccessStr = "Poll form fetched successfully";
        //private const string NoPollFormsStr = "Could not find forms with the specified Pollster ID";
        private const string PollFormsSuccessStr = "Poll forms fetched successfully";
        private const string PollRemovedStr = "Poll form removed successfully";
        private const string PollUpdatedStr = "Poll updated successfully";
        private const string PollCreatedStr = "Poll created successfully";

        private const string PrevPollNotFoundStr = "Could not find previous poll";

        private const string AccountMismatchStr = "You cannot modify this poll";
        //private const string NotCreatedStr = "Poll could not be created";

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

        /*public async Task<Response<GetPollFormDto>> GetPollForm(int pollId)
        {
            var response = new Response<GetPollFormDto>();
            var pollForm = await _context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr);

            var pollFormDto = _mapper.Map<GetPollFormDto>(pollForm);
            var questionsResponse = await GetQuestionsDto(pollForm, response, null);
            return questionsResponse.Data == null
                ? response.Failure(questionsResponse.Message)
                : response.Success(pollFormDto, PollFormSuccessStr);
        }*/

        public async Task<Response<List<GetPollFormDto>>> GetAllPollForms(bool archived)
        {
            var response = new Response<List<GetPollFormDto>>();
            var dtoPolls = await _context.PollForms.Select(p => _mapper.Map<GetPollFormDto>(p)).ToListAsync();
            dtoPolls = dtoPolls.Where(p => archived ? p.Archived : !p.Archived).ToList();

            foreach (GetPollFormDto getPollFormDto in dtoPolls)
            {
                var questionsResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
                getPollFormDto.Questions = questionsResponse.Data;
            }
            return response.Success(dtoPolls, PollFormsSuccessStr);
        }

        public async Task<Response<List<GetPollFormDto>>> GetUserPollForms(string email, bool filled)
        {
            var response = new Response<List<GetPollFormDto>>();
            var responseAll = await GetAllUserPollForms(email);
            if (responseAll.Data == null) return response.Failure(responseAll.Message);
            var polls = responseAll.Data;
            
            var accountId = (await GetAccount(email)).Data.AccountId;
            
            var filledPolls = (
                from poll in polls
                join question in _context.Questions on poll.PollId equals question.Poll
                join answer in _context.Answers on question.QuestionId equals answer.QuestionId
                where answer.AccountId == accountId
                select poll
            ).ToList();

            responseAll.Data = filled ? filledPolls : polls.Except(filledPolls).ToList();
            var dtoPolls = responseAll.Data.Select(p => _mapper.Map<GetPollFormDto>(p)).ToList();

            foreach (GetPollFormDto getPollFormDto in dtoPolls)
            {
                var questionsResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
                getPollFormDto.Questions = questionsResponse.Data;
            }

            return response.Success(dtoPolls, PollFormsSuccessStr);
        }

        private async Task<Response<List<PollForm>>> GetAllUserPollForms(string email)
        {
            var response = new Response<List<PollForm>>();
            var userResponse = await GetAccount(email);
            if (userResponse.Data == null) return response.Failure(userResponse.Message);

            var user = userResponse.Data;
            string[] tags = user.Tags.Split('/');

            var polls = await _context.PollForms
                .Where(p => p.Emails.Contains(email) || tags[0] != "") /*&& tags.Any(p.Tags.Contains))*/
                .ToListAsync();
            var pollsTagged = new List<PollForm>();
            foreach (PollForm pollForm in polls)
            {
                string[] pollTags = pollForm.Tags.Split('/');
                if (pollTags.Intersect(tags).Any()) pollsTagged.Add(pollForm);
            }

            return response.Success(pollsTagged, PollFormsSuccessStr);
        }

        public async Task<Response<List<GetPollFormDto>>> GetPollsterPollForms(string email, bool archived)
        {
            var response = await GetAllPollsterPollForms(email);
            if (response.Data == null) return response;

            var polls = response.Data.Where(p => archived ? p.Archived : !p.Archived).ToList();
            foreach (GetPollFormDto getPollFormDto in polls)
            {
                var questionsResponse = await _questionService.GetQuestions(getPollFormDto.PollId);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
                getPollFormDto.Questions = questionsResponse.Data;
            }

            response.Data = polls;
            return response;
        }

        private async Task<Response<List<GetPollFormDto>>> GetAllPollsterPollForms(string email)
        {
            var response = new Response<List<GetPollFormDto>>();
            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse.Message);

            var pollsterId = pollsterResponse.Data.AccountId;

            var pollForms = await _context.PollForms.Where(p => p.AuthorId == pollsterId).ToListAsync();
            if (pollForms == null) return response.Success(null, PollFormsSuccessStr);

            var pollFormsDto = new List<GetPollFormDto>();
            foreach (PollForm pollForm in pollForms)
            {
                var questionsResponse = await GetQuestionsDto(pollForm, response, pollFormsDto);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
            }

            return response.Success(pollFormsDto, PollFormsSuccessStr);
        }

        public async Task<Response<GetPollFormDto>> RemovePollForm(int pollId, string email)
        {
            var response = new Response<GetPollFormDto>();

            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse.Message);
            var account = pollsterResponse.Data;

            var pollForm = await _context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr);

            var condition = pollForm.AuthorId != account.AccountId && account.UserType != UserType.Admin;
            if (condition) return response.Failure(AccountMismatchStr);

            await _questionService.RemoveQuestions(pollId);
            _context.PollForms.Remove(pollForm);
            await _context.SaveChangesAsync();
            //await _statService.RemovePollStats(pollId);
            //await _statService.RemoveQuestionsStats(pollId);
            return response.Success(_mapper.Map<GetPollFormDto>(pollForm), PollRemovedStr);
        }

        public async Task<Response<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, string email)
        {
            var response = new Response<GetPollFormDto>();
            var previousForm = await _context.PollForms.FindAsync(pollForm.PreviousPollId);
            if (previousForm == null) return response.Failure(PrevPollNotFoundStr);

            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse.Message);
            var account = pollsterResponse.Data;
            bool fail = previousForm.AuthorId != account.AccountId && account.UserType != UserType.Admin;
            if (fail) return response.Failure(AccountMismatchStr);

            previousForm.Archived = true;
            var createPollFormDto = _mapper.Map<CreatePollFormDto>(pollForm);
            response = await CreatePollForm(createPollFormDto, email);

            if (response.Data == null) return response;
            response.Message = PollUpdatedStr;
            return response;
        }

        public async Task<Response<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, string email)
        {
            var response = new Response<GetPollFormDto>();
            var pollsterResponse = await GetAccount(email);
            if (pollsterResponse.Data == null) return response.Failure(pollsterResponse.Message);

            var noQuestions = pollForm.Questions == null || pollForm.Questions.Count == 0;
            if (noQuestions) return response.Failure(NoQuestionsStr);

            if (pollForm.Questions.Count != pollForm.Questions.Select(q => q.Position).Distinct().Count())
                return response.Failure(InvalidIndexStr);

            var dalForm = _mapper.Map<PollForm>(pollForm);
            dalForm.AuthorId = pollsterResponse.Data.AccountId;
            await _context.PollForms.AddAsync(dalForm);
            await _context.SaveChangesAsync();

            //int topIndex = await _context.PollForms.CountAsync(); HAŃBA
            var questions = new List<GetQuestionDto>();
            foreach (CreateQuestionDto createQuestionDto in pollForm.Questions)
            {
                var questionResponse = await _questionService.CreateQuestion(createQuestionDto, dalForm.PollId);
                if (questionResponse.Data == null) return response.Failure(questionResponse.Message);
                questions.Add(questionResponse.Data);
            }

            var pollResponse = await _statService.CreatePollStats(dalForm.PollId);
            if (pollResponse.Data == null) return response.Failure(pollResponse.Message);

            var getPollFormDto = _mapper.Map<GetPollFormDto>(dalForm);
            getPollFormDto.Questions = questions;

            await _statService.CreateQuestionsStats(getPollFormDto.Questions);

            return response.Success(getPollFormDto, PollCreatedStr);
        }

        private async Task<Response<T>> GetQuestionsDto<T>(
            PollForm updatePollFormDto,
            Response<T> response,
            ICollection<GetPollFormDto> questions)
        {
            questions?.Clear();
            GetPollFormDto pollFormDto = _mapper.Map<GetPollFormDto>(updatePollFormDto);
            var questionsResponse = await _questionService.GetQuestions(updatePollFormDto.PollId);
            if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
            pollFormDto.Questions = questionsResponse.Data;
            questions?.Last().Questions.AddRange(questionsResponse.Data);
            return response.Success(default, questionsResponse.Message);
        }

        //======================= HELPER METHODS ===================//

        private async Task<Response<Account>> GetAccount(string email)
        {
            var response = new Response<Account>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            return account == null ? response.Failure(AccountNotFoundStr) : response.Success(account, "");
        }
    }
}