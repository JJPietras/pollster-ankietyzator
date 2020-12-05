using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.AnswerDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IAnswerService// : IDbContextService
    {
        Task<Response<List<GetAnswerDto>>> GetAnswers(string email, int pollId, string authorMail);
        Task<Response<List<GetAnswerDto>>> AddAnswers(List<CreateAnswerDto> answers, string email);
    }
}