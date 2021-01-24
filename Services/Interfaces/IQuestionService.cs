using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IQuestionService// : IDbContextService
    {
        Task<ServiceResponse<List<GetQuestionDto>>> GetQuestions(int pollId);

        Task<ServiceResponse<GetQuestionDto>> CreateQuestion(CreateQuestionDto question, int pollId);

        Task<ServiceResponse<object>> RemoveQuestions(int pollId);
    }
}