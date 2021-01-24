using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.AnswerDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IAnswerService
    {
        Task<ServiceResponse<List<GetAnswerDto>>> GetAnswers(string email, int pollId, string authorMail);
        Task<ServiceResponse<List<GetAnswerDto>>> AddAnswers(List<CreateAnswerDto> answers, string email);
        Task<ServiceResponse<List<GetDetailedAnswerDto>>> GetDetailedAnswers(int pollId);
        Task<ServiceResponse<List<GetAnswerDto>>> GetAnonymousAnswers(int pollId);
    }
}