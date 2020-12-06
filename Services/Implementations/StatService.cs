﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private const string NoAccountStr = "Could not find specified account for provided Email";
        private const string NoPollStatStr = "Could not find specified poll stat for provided poll ID";
        private const string PollStatFetchedStr = "Poll stats fetched successfully";
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

        public StatService(AnkietyzatorDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<PollStat>> GetPollStat(int pollId)
        {
            var response = new ServiceResponse<PollStat>();
            var stats = await _context.PollStats.FindAsync(pollId);
            return stats == null
                ? response.Failure(NoPollStatStr, HttpStatusCode.NotFound)
                : response.Success(stats, PollStatFetchedStr);
        }

        public async Task<ServiceResponse<List<PollStat>>> GetPollsStats(string pollsterEmail)
        {
            var response = new ServiceResponse<List<PollStat>>();
            const HttpStatusCode code = HttpStatusCode.NotFound;
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == pollsterEmail);
            if (account == null) return response.Failure(NoAccountStr, code);

            var polls = await _context.PollForms
                .Where(p => p.AuthorId == account.AccountId)
                .Select(p => p.PollId)
                .ToListAsync();

            if (polls.Count == 0) return response.Failure(NoPollsStr, code);
            var pollStats = await _context.PollStats
                .Where(p => polls.Contains(p.PollId))
                .ToListAsync();

            return pollStats.Count == 0 || pollStats.Count != polls.Count
                ? response.Failure(NoPollStatsStr, code)
                : response.Success(pollStats, PollStatsFetchedStr);
        }

        public async Task<ServiceResponse<List<QuestionStat>>> GetQuestionsStats(int pollId)
        {
            var response = new ServiceResponse<List<QuestionStat>>();
            var questions = await _context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();

            if (questions.Count == 0) return response.Failure(PollNoQuestionsStr, HttpStatusCode.Conflict);
            var questionStats = await _context.QuestionStats
                .Where(q => questions.Contains(q.QuestionId))
                .ToListAsync();

            return questionStats.Count == 0 || questionStats.Count != questions.Count
                ? response.Failure(NoQuestionStatsStr, HttpStatusCode.NotFound)
                : response.Success(questionStats, QuestionStatsFetchedStr);
        }

        public async Task<ServiceResponse<PollStat>> CreatePollStats(int pollId)
        {
            var response = new ServiceResponse<PollStat>();
            if (await _context.PollForms.FindAsync(pollId) == null) 
                return response.Failure(NoPollWithIdString, HttpStatusCode.NotFound);
            var stats = new PollStat
            {
                PollId = pollId,
                Completions = 0,
                Percentage = 0f
            };
            await _context.PollStats.AddAsync(stats);
            await _context.SaveChangesAsync();
            return response.Success(stats, PollStatCreatedStr);
        }

        public async Task<ServiceResponse<List<QuestionStat>>> CreateQuestionsStats(List<GetQuestionDto> questions)
        {
            var response = new ServiceResponse<List<QuestionStat>>();
            var questionStats = questions
                .Select(q => new QuestionStat {QuestionId = q.QuestionId, AnswerCounts = GetOptionsCount(q.Options)})
                .ToList();
            await _context.QuestionStats.AddRangeAsync(questionStats);
            await _context.SaveChangesAsync();
            return response.Success(questionStats, QuestionStatsCreatedStr);
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

        private static string GetOptionsCount(string options)
        {
            int count = options.Split('/').Length;
            var builder = new StringBuilder().Insert(0, "0/", count);
            return builder.Remove(builder.Length - 1, 1).ToString();
        }
    }
}