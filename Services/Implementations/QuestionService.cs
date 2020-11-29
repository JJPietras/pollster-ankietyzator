using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DTO.QuestionDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private const string NoQuestionsStr = "There are no questions for the selected poll in the database";
        private const string QuestionsSuccessStr = "Questions fetched successfuly";
        private const string QuestionsRemovedStr = "Questions removed successfuly";
        private const string QuestionCreatedStr = " Question created successfuly";
        
        public AnkietyzatorDbContext Context { get; set; }
        private readonly IMapper _mapper;

        public QuestionService(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        public async Task<Response<List<GetQuestionDto>>> GetQuestions(int pollId)
        {
            var response = new Response<List<GetQuestionDto>>();
            var questions = await Context.Questions.Where(q => q.Poll == pollId).ToListAsync();
            if (questions.Count == 0) return response.Failure(NoQuestionsStr);
            var questionsDto = questions.Select(q => _mapper.Map<GetQuestionDto>(q)).ToList();
            return response.Success(questionsDto, QuestionsSuccessStr);
        }

        public async Task<Response<GetQuestionDto>> CreateQuestion(CreateQuestionDto question, int pollId)
        {
            var dalQuestion = _mapper.Map<Question>(question);
            dalQuestion.Poll = pollId;
            await Context.Questions.AddAsync(dalQuestion);
            await Context.SaveChangesAsync();
            var getQuestionDto = _mapper.Map<GetQuestionDto>(dalQuestion);
            return new Response<GetQuestionDto>().Success(getQuestionDto, QuestionCreatedStr);
        }

        public async Task<Response<object>> RemoveQuestions(int pollId)
        {
            var response = new Response<object>();
            var questions = await Context.Questions.Where(q => q.Poll == pollId).ToListAsync();
            if (questions.Count == 0) return response.Failure(NoQuestionsStr);
            Context.Questions.RemoveRange(questions);
            await Context.SaveChangesAsync();
            return response.Success(null, QuestionsRemovedStr);
        }
    }
}