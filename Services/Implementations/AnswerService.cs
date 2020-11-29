using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DTO.AnswerDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class AnswerService : IAnswerService
    {
        private const string AnswersFetchedStr = "Answers fetched successfuly";
        private const string AnswersCreatedStr = "Answers created successfuly";
        
        public AnkietyzatorDbContext Context { get; set; }
        private readonly IMapper _mapper;

        public AnswerService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Response<List<GetAnswerDto>>> GetAnswers(int userId, int pollId)
        {
            var questions = await Context.Questions
                .Where(q => q.Poll == pollId)
                .Select(q => q.QuestionId)
                .ToListAsync();
            var answers = await Context.Answers
                .Where(a => a.AccountId == userId && questions.Contains(a.QuestionId))
                .Select(q => _mapper.Map<GetAnswerDto>(q))
                .ToListAsync();
            return new Response<List<GetAnswerDto>>().Success(answers, AnswersFetchedStr);
        }

        public async Task<Response<List<GetAnswerDto>>> CreateAnswers(List<CreateAnswerDto> answers, int userId)
        {
            var dalAnswers = answers.Select(a => _mapper.Map<Answer>(a)).ToList();
            foreach (Answer dalAnswer in dalAnswers) dalAnswer.AccountId = userId;
            await Context.Answers.AddRangeAsync(dalAnswers);
            await Context.SaveChangesAsync();
            var getAnswers = dalAnswers.Select(a => _mapper.Map<GetAnswerDto>(a)).ToList();
            return new Response<List<GetAnswerDto>>().Success(getAnswers, AnswersCreatedStr);
        }
    }
}