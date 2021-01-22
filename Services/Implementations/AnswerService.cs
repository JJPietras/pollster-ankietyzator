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
using Ankietyzator.Models.DTO.AnswerDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class AnswerService : IAnswerService
    {
        private const string BaseUrl = "https://cc-2020-group-one-ankietyzator-function.azurewebsites.net/api/";
        private const string AddAnswer = "AddAnswer?code=lLPk2eq41HV0miGWEaZr6JoHRKDhMWZAWuEXpUpfrl6z8ydvumOwxA==";
        private const string Update = "UpdatePollStats?code=Et1tL0adY4uEKmeS3pK5/I9WzGf0BznFAnVO3CdtvUNYY30K5fLYLA==";
        
        private const string AccountNotFoundStr = "Could not find account";
        private const string AuthorNotFoundStr = "Could not find author account";
        private const string AnswersFetchedStr = "Answers fetched successfully";
        private const string AnswersCreatedStr = "Answers created successfully";
        private const string AnswersDetailedInfoNotAvailable = "Cannot retrieve detailed answers for an anonymous Poll";
        private const string AlreadyAnsweredStr = "User has already answered at least one of provided questions";
        private const string NoQuestionStr = "Provided question does not belong to any poll";
        private const string AnswersForManyStr = "Answers asociated with questions that belongs to multiple polls";
        private const string InvalidQuestionsStr = "The answer count is other than question count. Possible duplicates";


        private readonly AnkietyzatorDbContext _context;
        private readonly IMapper _mapper;

        public AnswerService(AnkietyzatorDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetAnswerDto>>> GetAnswers(string email, int pollId, string authorMail)
        {
            var response = new ServiceResponse<List<GetAnswerDto>>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null) return response.Failure(AccountNotFoundStr, HttpStatusCode.NotFound);

            bool authorized = false;
            if (!string.IsNullOrEmpty(authorMail))
            {
                var author = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == authorMail);
                if (author == null) return response.Failure(AuthorNotFoundStr, HttpStatusCode.NotFound);
                authorized = await _context.PollForms.FirstOrDefaultAsync(
                    p => p.AuthorId == author.AccountId && p.PollId == pollId) != null;
                authorized = authorized || author.UserType == UserType.Admin;
            }

            var questions = await _context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();
            var answers = await _context.Answers
                .Where(a => (a.AccountId == account.AccountId || authorized) && questions.Contains(a.QuestionId))
                .Select(q => _mapper.Map<GetAnswerDto>(q))
                .ToListAsync();
            return response.Success(answers, AnswersFetchedStr);
        }

        public async Task<ServiceResponse<List<GetDetailedAnswerDto>>> GetDetailedAnswers(int pollId){
            var response = new ServiceResponse<List<GetDetailedAnswerDto>>();
            //var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            //if (account == null) return response.Failure(AccountNotFoundStr, HttpStatusCode.NotFound);
            
            var nonAnymous = await _context.PollForms.Where(p => pollId == p.PollId).Select(q => q.NonAnonymous).FirstOrDefaultAsync();
            if (!nonAnymous) return response.Failure(AnswersDetailedInfoNotAvailable, HttpStatusCode.BadRequest);

            var usersAnswered = (from a in _context.Answers 
                                 join q in _context.Questions on a.QuestionId equals q.QuestionId 
                                 where q.Poll == pollId select a.AccountId).Distinct();

            var answers = await _context.Accounts
                .Where(a => usersAnswered.Contains(a.AccountId))
                .Select(q => _mapper.Map<GetDetailedAnswerDto>(q))
                .ToListAsync();

            answers.ForEach(a => a.Answers = (from qa in _context.Answers 
                                              join p in _context.Questions on qa.QuestionId equals p.QuestionId
                                              where qa.AccountId == a.AccountId && p.Poll == pollId 
                                              select _mapper.Map<GetAnswerDto>(qa)).ToList());
            return response.Success(answers, AnswersFetchedStr);
        }
        public async Task<ServiceResponse<List<GetAnswerDto>>> GetAnonymousAnswers(int pollId){
            var response = new ServiceResponse<List<GetAnswerDto>>();

            var answers = await _context.Answers
                .Where(a => _context.Questions.Any(q => q.QuestionId == a.QuestionId && q.Poll == pollId && q.Type==3))
                .Select(ans => _mapper.Map<GetAnswerDto>(ans))
                .ToListAsync();

            return response.Success(answers, AnswersFetchedStr);
        }

        public async Task<ServiceResponse<List<GetAnswerDto>>> AddAnswers(List<CreateAnswerDto> answers, string email)
        {
            var response = new ServiceResponse<List<GetAnswerDto>>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null) return response.Failure(AccountNotFoundStr, HttpStatusCode.NotFound);

            // First we take all question IDs from answer list
            var questionIDs = answers.Select(a => a.QuestionId).ToList();

            // Then we take questions that have previously acquired IDs and select poll IDs of these questions
            var pollIDs = await _context.Questions
                .Where(q => questionIDs.Contains(q.QuestionId))
                .Select(q => q.Poll)
                .Distinct()
                .ToListAsync();

            // If there are more than one poll IDs associated with questions or none - return failure  
            if (pollIDs.Count > 1) return response.Failure(AnswersForManyStr, HttpStatusCode.UnprocessableEntity);
            if (pollIDs.Count == 0) return response.Failure(NoQuestionStr, HttpStatusCode.UnprocessableEntity);

            // Then we take all questions that are bound to this poll to check if there are all questions in answers
            // var pollQuestionIDs = await _context.Questions
            //     .Where(q => q.Poll == pollIDs.ElementAtOrDefault(0))
            //     .Select(q => q.QuestionId)
            //     .ToListAsync();

            // // After that we have to heck if amount of questions and answers are equal
            // if (!answers.Select(a => a.QuestionId).Distinct().SequenceEqual(pollQuestionIDs))
            //     return response.Failure(InvalidQuestionsStr, HttpStatusCode.UnprocessableEntity); //It had to be changed to account for the non obligatory questions

            var requiredQuestions = await _context.Questions
                .Where(q => q.Poll == pollIDs.FirstOrDefault() && !q.AllowEmpty)
                .Select(q => q.QuestionId)
                .ToListAsync();

            var requiredAnswers = answers
                .Where(a => requiredQuestions.Any(q => q == a.QuestionId ) )
                .Select(a => a.QuestionId).ToList();

            if (!requiredAnswers.SequenceEqual(requiredQuestions))
                return response.Failure(InvalidQuestionsStr, HttpStatusCode.UnprocessableEntity);

            // Lastly we have to check if there are any already answered questions by the current user
            var dalAnswers = answers.Select(a => _mapper.Map<Answer>(a)).ToList();
            foreach (Answer dalAnswer in dalAnswers)
            {
                dalAnswer.AccountId = account.AccountId;
                var answer = await _context.Answers.FindAsync(dalAnswer.AccountId, dalAnswer.QuestionId);
                if (answer != null) return response.Failure(AlreadyAnsweredStr, HttpStatusCode.Conflict);
            }

            var getAnswers = dalAnswers.Select(a => _mapper.Map<GetAnswerDto>(a)).ToList();
            await _context.Answers.AddRangeAsync(dalAnswers);
            await _context.SaveChangesAsync();

            RunFunctions(pollIDs[0], account.AccountId);

            return response.Success(getAnswers, AnswersCreatedStr);
        }

        private static async void RunFunctions(int pollId, int accountId)
        {
            using var httpClient = new HttpClient();
            
            // Update poll stats
            var pollBuilder = new StringBuilder(BaseUrl).Append(Update);
            pollBuilder.Append($"&accountId={accountId}");
            var pollMessage = new HttpRequestMessage(HttpMethod.Get, pollBuilder.ToString()); 
            
            // Update question stats
            var questionBuilder = new StringBuilder(BaseUrl).Append(AddAnswer);
            questionBuilder.Append($"&pollId={pollId}&accountId={accountId}");
            Console.WriteLine(questionBuilder.ToString());
            var questionMessage = new HttpRequestMessage(HttpMethod.Get, questionBuilder.ToString());

            var pollResponse = await httpClient.SendAsync(pollMessage);
            var questionResponse = await httpClient.SendAsync(questionMessage);

            dynamic pollResult = await pollResponse.Content.ReadAsStringAsync();
            dynamic questionResult = await questionResponse.Content.ReadAsStringAsync();
            
            Console.WriteLine("Poll response: " + pollResult);
            Console.WriteLine("Question response: " + questionResult);
        }
    }
}