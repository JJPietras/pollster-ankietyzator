using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.QuestionDTOs;
using Ankietyzator.Models.DTO.StatsDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class StatService : IStatService
    {
        private const string NoAccountStr = "Could not find specified account for provided Email";
        private const string NoPollStatStr = "Could not find specified poll stat for provided poll ID";
        private const string PollStatFetchedStr = "Poll stats fetched successfully";
        private const string NoPoll = "Could not find specified poll";
        private const string NoPollsStr = "User has no polls";
        private const string NoPollStatsStr = "Could not find (all) stats";
        private const string PollStatsFetchedStr = "Polls stats fetched successfully";
        private const string PollNoQuestionsStr = "Poll does not have any questions";
        private const string QuestionStatsFetchedStr = "Question stats fetched successfully";
        private const string NoQuestionStatsStr = "Could not find (all) question stats";
        private const string NoPollWithIdString = "Poll with provided poll ID does not exists";
        private const string PollStatCreatedStr = "Poll stats created successfully";

        private const string QuestionStatsCreatedStr = "Question stats created successfully";
        /*private const string PollStatNotFoundStr = "Could not found poll stat for provided ID";
        private const string PollStatRemovedStr = "Poll stats removed successfully";
        private const string NoQuestionsStr = "Could not find questions";
        private const string QuestionsStatsRemovedStr = "Questions stats removed successfully";*/

        private readonly AnkietyzatorDbContext _context;
        private readonly IMapper _mapper;

        public StatService(AnkietyzatorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetPollStatsDto>> GetPollStat(int pollId)
        {
            var response = new ServiceResponse<GetPollStatsDto>();

            var pollForm = await _context.PollForms.FindAsync(pollId);
            if (pollForm == null) return response.Failure(NoPollsStr, HttpStatusCode.NotFound);

            var stats = await _context.PollStats.FindAsync(pollId);
            if (stats == null) response.Failure(NoPoll, HttpStatusCode.NotFound);

            var pollStats = _mapper.Map<GetPollStatsDto>(stats);
            (pollStats.Title, pollStats.Description) = (pollForm.Title, pollForm.Description);
            return response.Success(pollStats, PollStatFetchedStr);
        }

        public async Task<ServiceResponse<List<GetPollStatsDto>>> GetPollsStats(string pollsterEmail)
        {
            var response = new ServiceResponse<List<GetPollStatsDto>>();
            const HttpStatusCode code = HttpStatusCode.NotFound;
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == pollsterEmail);
            if (account == null) return response.Failure(NoAccountStr, code);

            var polls = await _context.PollForms
                .Where(p => p.AuthorId == account.AccountId)
                .ToListAsync();

            var pollsId = polls.Select(p => p.PollId).ToList();

            if (polls.Count == 0) return response.Success(new List<GetPollStatsDto>(), PollStatsFetchedStr);
            var pollStats = await _context.PollStats
                .Where(p => pollsId.Contains(p.PollId))
                .ToListAsync();

            if (pollStats.Count == 0 || pollStats.Count != polls.Count)
                return response.Failure(NoPollStatsStr, code);

            var pollStatsDto = pollStats.Select(s => _mapper.Map<GetPollStatsDto>(s)).ToList();
            for (int i = 0; i < pollStats.Count; i++)
            {
                pollStatsDto[i].Title = polls[i].Title;
                pollStatsDto[i].Description = polls[i].Description;
            }

            return response.Success(pollStatsDto, PollStatsFetchedStr);
        }

        public async Task<ServiceResponse<List<GetQuestionStatsDto>>> GetQuestionsStats(int pollId)
        {
            var response = new ServiceResponse<List<GetQuestionStatsDto>>();
            var questions = await _context.Questions
                .Where(q => q.Poll == pollId)
                .ToListAsync();

            var questionsId = questions.Select(q => q.QuestionId).ToList();

            if (questions.Count == 0) return response.Failure(PollNoQuestionsStr, HttpStatusCode.Conflict);
            var questionStats = await _context.QuestionStats
                .Where(q => questionsId.Contains(q.QuestionId))
                .ToListAsync();

            if (questionStats.Count == 0 || questionStats.Count != questions.Count)
                return response.Failure(NoQuestionStatsStr, HttpStatusCode.NotFound);

            var questionStatsDto = questionStats.Select(q => _mapper.Map<GetQuestionStatsDto>(q)).ToList();
            for (int i = 0; i < questions.Count; i++) UpdateGetQuestionStatDto(questionStatsDto[i], questions[i]);
            return response.Success(questionStatsDto, QuestionStatsFetchedStr);
        }

        public async Task<ServiceResponse<GetPollStatsDto>> CreatePollStats(int pollId)
        {
            var response = new ServiceResponse<GetPollStatsDto>();
            var poll = await _context.PollForms.FindAsync(pollId);
            if (poll == null) return response.Failure(NoPollWithIdString, HttpStatusCode.NotFound);
            var stats = new PollStat
            {
                PollId = pollId,
                Completions = 0,
                Percentage = 0f
            };
            await _context.PollStats.AddAsync(stats);
            await _context.SaveChangesAsync();

            var pollStats = _mapper.Map<GetPollStatsDto>(stats);
            (pollStats.Title, pollStats.Description) = (poll.Title, poll.Description);
            return response.Success(pollStats, PollStatCreatedStr);
        }

        public async Task<ServiceResponse<List<GetQuestionStatsDto>>> CreateQuestionsStats(
            List<GetQuestionDto> questions)
        {
            var response = new ServiceResponse<List<GetQuestionStatsDto>>();
            var questionStats = questions
                .Select(q => new QuestionStat
                {
                    QuestionId = q.QuestionId, 
                    AnswerCounts = GetOptionsCount(q.Options, q.Type)
                })
                .ToList();
            await _context.QuestionStats.AddRangeAsync(questionStats);
            await _context.SaveChangesAsync();

            var questionStatsDto = questionStats.Select(q => _mapper.Map<GetQuestionStatsDto>(q)).ToList();
            var dalQuestions = questions.Select(q => _mapper.Map<Question>(q)).ToList();
            for (int i = 0; i < questions.Count; i++) UpdateGetQuestionStatDto(questionStatsDto[i], dalQuestions[i]);

            return response.Success(questionStatsDto, QuestionStatsCreatedStr);
        }

        /*public async Task<Response<object>> RemovePollStats(int pollId)
        {
            var response = new Response<object>();
            var pollStat = await _context.PollStats.FindAsync(pollId);
            if (pollStat == null) return response.Failure(PollStatNotFoundStr);
            _context.PollStats.Remove(pollStat);
            await _context.SaveChangesAsync();
            return response.Success(null, PollStatRemovedStr);
        }

        public async Task<Response<object>> RemoveQuestionsStats(int pollId)
        {
            var response = new Response<object>();
            var questions = await _context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();

            if (questions.Count == 0) return response.Failure(NoQuestionsStr);
            var questionStats = await _context.QuestionStats
                .Where(q => questions.Contains(q.QuestionId))
                .ToListAsync();
            
            _context.QuestionStats.RemoveRange(questionStats);
            await _context.SaveChangesAsync();

            return questionStats.Count == 0 || questionStats.Count != questions.Count
                ? response.Failure(NoQuestionStatsStr)
                : response.Success(null, QuestionsStatsRemovedStr);
        }*/

        private static void UpdateGetQuestionStatDto(GetQuestionStatsDto dto, Question dal)
        {
            (dto.Title, dto.Options, dto.Position) = (dal.Title, dal.Options, dal.Position);
            (dto.Type, dto.AllowEmpty, dto.MaxLength) = (dal.Type, dal.AllowEmpty, dal.MaxLength);
        }

        private static string GetOptionsCount(string options, QuestionType type)
        {
            if (type == QuestionType.Number) return "";
            if (type == QuestionType.Text) return "0";
            int count = options.Split('/').Length;
            var builder = new StringBuilder().Insert(0, "0/", count);
            return builder.Remove(builder.Length - 1, 1).ToString();
        }
    }
}