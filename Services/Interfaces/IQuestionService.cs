using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IQuestionService : IDbContextService
    {
        Task<Response<List<GetQuestionDto>>> GetQuestions(int pollId);

        Task<Response<GetQuestionDto>> CreateQuestion(CreateQuestionDto question, int pollId);

        Task<Response<object>> RemoveQuestions(int pollId);
    }
}