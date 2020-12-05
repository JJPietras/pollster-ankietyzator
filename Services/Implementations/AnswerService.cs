using System.Collections.Generic;
using System.Linq;
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
        private const string AccountNotFoundStr = "Could not find account";
        private const string AuthorNotFoundStr = "Could not find author account";
        private const string AnswersFetchedStr = "Answers fetched successfully";
        private const string AnswersCreatedStr = "Answers created successfully";

        private const string AlreadyAnsweredStr = "User has already answered at least one of provided questions";
        private const string AnswersForManyStr = "Answers asociated with questions that belongs to multiple polls";
        private const string InvalidQuestionsStr = "The answer count is other than question count. Possible duplicates";

        private readonly AnkietyzatorDbContext _context;
        private readonly IMapper _mapper;

        public AnswerService(AnkietyzatorDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Response<List<GetAnswerDto>>> GetAnswers(string email, int pollId, string authorMail)
        {
            var response = new Response<List<GetAnswerDto>>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null) return response.Failure(AccountNotFoundStr);

            bool authorized = false;
            if (!string.IsNullOrEmpty(authorMail))
            {
                var author = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == authorMail);
                if (author == null) return response.Failure(AuthorNotFoundStr);
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

        public async Task<Response<List<GetAnswerDto>>> AddAnswers(List<CreateAnswerDto> answers, string email)
        {
            var response = new Response<List<GetAnswerDto>>();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null) return response.Failure(AccountNotFoundStr);

            // First we take all question IDs from answer list
            var questionIDs = answers.Select(a => a.QuestionId).ToList();
            
            // Then we take questions that have previously acquired IDs and select poll IDs of these questions
            var pollIDs = await _context.Questions
                .Where(q => questionIDs.Contains(q.QuestionId))
                .Select(q => q.Poll)
                .Distinct()
                .ToListAsync();

            // If there are more than one poll IDs associated with questions - return failure  
            if (pollIDs.Count > 1) return response.Failure(AnswersForManyStr);

            // Then we take all questions that are bound to this poll to check if there are all questions in answers
            var pollQuestionIDs = await _context.Questions
                .Where(q => q.Poll == pollIDs[0])
                .Select(q => q.QuestionId)
                .ToListAsync();

            // After that we have to heck if amount of questions and answers are equal
            if (answers.Select(a => a.QuestionId).Distinct().Count() != pollQuestionIDs.Count)
                return response.Failure(InvalidQuestionsStr);
            
            // Lastly we have to check if there are any already answered questions by the current user
            var dalAnswers = answers.Select(a => _mapper.Map<Answer>(a)).ToList();
            foreach (Answer dalAnswer in dalAnswers)
            {
                dalAnswer.AccountId = account.AccountId;
                var answer = await _context.Answers.FindAsync(dalAnswer.AccountId, dalAnswer.QuestionId);
                if (answer != null) return response.Failure(AlreadyAnsweredStr);
            }
            
            var getAnswers = dalAnswers.Select(a => _mapper.Map<GetAnswerDto>(a)).ToList();
            await _context.Answers.AddRangeAsync(dalAnswers);
            await _context.SaveChangesAsync();
            return response.Success(getAnswers, AnswersCreatedStr);
        }
    }
}