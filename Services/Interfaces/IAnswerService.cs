using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.AnswerDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IAnswerService// : IDbContextService
    {
        Task<Response<List<GetAnswerDto>>> GetAnswers(int userId, int pollId);
        Task<Response<List<GetAnswerDto>>> CreateAnswers(List<CreateAnswerDto> answers, int userId);
    }
}