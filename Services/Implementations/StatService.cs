using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.QuestionDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class StatService : IStatService
    {
        private const string NoPollStatStr = "Could not find specified poll stat for provided poll ID";
        private const string PollStatFetchedStr = "Poll stats fetched successfuly";
        private const string NoPollsStr = "User has no polls";
        private const string NoPollStatsStr = "Could not find (all) stats";
        private const string PollStatsFetchedStr = "Polls stats fetched successfuly";
        private const string PollNoQuestionsStr = "Poll does not have any questions";
        private const string QuestionStatsFetchedStr = "Question stats fetched successfuly";
        private const string NoQuestionStatsStr = "Could not find (all) question stats";
        private const string NoPollWithIdString = "Poll with provided poll ID does not exists";
        private const string PollStatCreatedStr = "Poll stats created successfuly";
        private const string QuestionStatsCreatedStr = "Question stats created successfuly";
        private const string PollStatNotFoundStr = "Could not found poll stat for provided ID";
        private const string PollStatRemovedStr = "Poll stats removed successfuly";
        private const string NoQuestionsStr = "Could not find questions";
        private const string QuestionsStatsRemovedStr = "Questions stats removed successfuly";

        public AnkietyzatorDbContext Context { get; set; }

        public async Task<Response<PollStat>> GetPollStat(int pollId)
        {
            var response = new Response<PollStat>();
            var stats = await Context.PollStats.FindAsync(pollId);
            return stats == null ? response.Failure(NoPollStatStr) : response.Success(stats, PollStatFetchedStr);
        }

        public async Task<Response<List<PollStat>>> GetPollsStats(int pollsterId)
        {
            var response = new Response<List<PollStat>>();
            var polls = await Context.PollForms
                .Where(p => p.AuthorId == pollsterId)
                .Select(p => p.PollId)
                .ToListAsync();

            if (polls.Count == 0) return response.Failure(NoPollsStr);
            var pollStats = await Context.PollStats
                .Where(p => polls.Contains(p.PollId))
                .ToListAsync();

            return pollStats.Count == 0 || pollStats.Count != polls.Count
                ? response.Failure(NoPollStatsStr)
                : response.Success(pollStats, PollStatsFetchedStr);
        }

        public async Task<Response<List<QuestionStat>>> GetQuestionsStats(int pollId)
        {
            var response = new Response<List<QuestionStat>>();
            var questions = await Context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();

            if (questions.Count == 0) return response.Failure(PollNoQuestionsStr);
            var questionStats = await Context.QuestionStats
                .Where(q => questions.Contains(q.QuestionId))
                .ToListAsync();

            return questionStats.Count == 0 || questionStats.Count != questions.Count
                ? response.Failure(NoQuestionStatsStr)
                : response.Success(questionStats, QuestionStatsFetchedStr);
        }

        public async Task<Response<PollStat>> CreatePollStats(int pollId)
        {
            var response = new Response<PollStat>();
            if (await Context.PollForms.FindAsync(pollId) == null) return response.Failure(NoPollWithIdString);
            var stats = new PollStat
            {
                PollId = pollId,
                Completions = 0,
                Percentage = 0f
            };
            await Context.PollStats.AddAsync(stats);
            await Context.SaveChangesAsync();
            return response.Success(stats, PollStatCreatedStr);
        }

        public async Task<Response<List<QuestionStat>>> CreateQuestionsStats(List<GetQuestionDto> questions)
        {
            var response = new Response<List<QuestionStat>>();
            var questionStats = questions
                .Select(q => new QuestionStat {QuestionId = q.QuestionId, AnswerCounts = GetOptionsCount(q.Options)})
                .ToList();
            await Context.QuestionStats.AddRangeAsync(questionStats);
            await Context.SaveChangesAsync();
            return response.Success(questionStats, QuestionStatsCreatedStr);
        }

        public async Task<Response<object>> RemovePollStats(int pollId)
        {
            var response = new Response<object>();
            var pollStat = await Context.PollStats.FindAsync(pollId);
            if (pollStat == null) return response.Failure(PollStatNotFoundStr);
            Context.PollStats.Remove(pollStat);
            await Context.SaveChangesAsync();
            return response.Success(null, PollStatRemovedStr);
        }

        public async Task<Response<object>> RemoveQuestionsStats(int pollId)
        {
            var response = new Response<object>();
            var questions = await Context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();

            if (questions.Count == 0) return response.Failure(NoQuestionsStr);
            var questionStats = await Context.QuestionStats
                .Where(q => questions.Contains(q.QuestionId))
                .ToListAsync();
            
            Context.QuestionStats.RemoveRange(questionStats);
            await Context.SaveChangesAsync();

            return questionStats.Count == 0 || questionStats.Count != questions.Count
                ? response.Failure(NoQuestionStatsStr)
                : response.Success(null, QuestionsStatsRemovedStr);
        }

        private static string GetOptionsCount(string options)
        {
            int count = options.Split('/').Length;
            var builder = new StringBuilder().Insert(0, "0/", count);
            return builder.Remove(builder.Length - 1, 1).ToString();
        }
    }
}