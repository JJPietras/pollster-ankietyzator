using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.PollDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IPollingService
    {
        //Task<Response<GetPollFormDto>> GetPollForm(int pollId);
        
        Task<ServiceResponse<List<GetPollFormDto>>> GetAllPollForms(bool archived);

        Task<ServiceResponse<List<GetPollFormDto>>> GetUserPollForms(string email, bool filled);
        
        Task<ServiceResponse<List<GetPollFormDto>>> GetPollsterPollForms(string email, bool archived);

        Task<ServiceResponse<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, string email);
        
        Task<ServiceResponse<GetPollFormDto>> RemovePollForm(int pollId, string email);

        Task<ServiceResponse<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, string email);
    }
}