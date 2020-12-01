using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
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
        private const string NoPollFormStr = "Could not find form with the specified ID";
        private const string PollFormSuccessStr = "Poll form fetched successfuly";
        private const string NoPollFormsStr = "Could not find forms with the specified Pollster ID";
        private const string PollFormsSuccessStr = "Poll forms fetched successfuly";
        private const string PollRemovedStr = "Poll form removed successfuly";
        private const string PollUpdatedStr = "Poll updated successfuly";
        private const string PollCreatedStr = "Poll created successfuly";
        private const string PrevPollNotFoundStr = "Could not find previous poll";
        //private const string NotCreatedStr = "Poll could not be created";

        public AnkietyzatorDbContext Context { get; set; }
        private readonly IQuestionService _questionService;
        private readonly IStatService _statService;
        private readonly IMapper _mapper;

        public PollingService(IMapper mapper, IQuestionService questionService, IStatService statService)
        {
            _questionService = questionService;
            _statService = statService;
            _mapper = mapper;
        }

        public void InitializeServicesContext(AnkietyzatorDbContext context ){
            _questionService.Context = context;
            _statService.Context = context;
        }

        public async Task<Response<GetPollFormDto>> GetPollForm(int pollId)
        {
            var response = new Response<GetPollFormDto>();
            var pollForm = await Context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr);
            
            var pollFormDto = _mapper.Map<GetPollFormDto>(pollForm);
            var questionsResponse = await GetQuestionsDto(pollForm, response, null);
            return questionsResponse.Data == null
                ? response.Failure(questionsResponse.Message)
                : response.Success(pollFormDto, PollFormSuccessStr);
        }

        public async Task<Response<List<GetPollFormDto>>> GetPollForms(int pollsterId)
        {
            var response = new Response<List<GetPollFormDto>>();
            var pollForms = await Context.PollForms.Where(p => p.AuthorId == pollsterId).ToListAsync();
            if (pollForms == null) return response.Failure(NoPollFormsStr);
            
            var pollFormsDto = new List<GetPollFormDto>();
            foreach (PollForm pollForm in pollForms)
            {
                var questionsResponse = await GetQuestionsDto(pollForm, response, pollFormsDto);
                if (questionsResponse.Data == null) return response.Failure(questionsResponse.Message);
            }

            return response.Success(pollFormsDto, PollFormsSuccessStr);
        }

        public async Task<Response<List<GetPollFormDto>>> GetArchivedPollForms(int pollsterId)
        {
            var pollsResponse = await GetPollForms(pollsterId);
            if (pollsResponse.Data == null) return pollsResponse;
            pollsResponse.Data = pollsResponse.Data.Where(p => p.Archived).ToList();
            return pollsResponse;
        }

        public async Task<Response<List<GetPollFormDto>>> GetNotArchivedPollForms(int pollsterId)
        {
            var pollsResponse = await GetPollForms(pollsterId);
            if (pollsResponse.Data == null) return pollsResponse;
            pollsResponse.Data = pollsResponse.Data.Where(p => !p.Archived).ToList();
            return pollsResponse;
        }

        public async Task<Response<GetPollFormDto>> RemovePollForm(int pollId)
        {
            var response = new Response<GetPollFormDto>();
            var pollForm = await Context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollFormStr);
            await _questionService.RemoveQuestions(pollId);
            Context.PollForms.Remove(pollForm);
            await Context.SaveChangesAsync();
            await _statService.RemovePollStats(pollId);
            await _statService.RemoveQuestionsStats(pollId);
            return response.Success(_mapper.Map<GetPollFormDto>(pollForm), PollRemovedStr);
        }

        public async Task<Response<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, int accountId)
        {
            var response = new Response<GetPollFormDto>();
            var previousForm = await Context.PollForms.FindAsync(pollForm.PreviousPollId);
            if (previousForm == null) return response.Failure(PrevPollNotFoundStr);
            previousForm.Archived = true;
            var createPollFormDto = _mapper.Map<CreatePollFormDto>(pollForm);
            response = await CreatePollForm(createPollFormDto, accountId);
            if (response.Data != null) response.Message = PollUpdatedStr;
            return response;
        }

        public async Task<Response<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, int accountId)
        {
            var response = new Response<GetPollFormDto>();
            
            var dalForm = _mapper.Map<PollForm>(pollForm);
            dalForm.AuthorId = accountId;
            await Context.PollForms.AddAsync(dalForm);
            await Context.SaveChangesAsync();
            
            int topIndex = await Context.PollForms.CountAsync();
            foreach (CreateQuestionDto createQuestionDto in pollForm.Questions)
            {
                await _questionService.CreateQuestion(createQuestionDto, dalForm.PollId);
            }

            var pollResponse = await _statService.CreatePollStats(dalForm.PollId);
            if (pollResponse.Data == null) return response.Failure(pollResponse.Message);
            
            var getPollFormDto = _mapper.Map<GetPollFormDto>(dalForm);
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
    }
}