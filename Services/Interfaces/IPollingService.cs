using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.PollDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IPollingService
    {
        //Task<Response<GetPollFormDto>> GetPollForm(int pollId);
        
        Task<Response<List<GetPollFormDto>>> GetAllPollForms(bool archived);

        Task<Response<List<GetPollFormDto>>> GetUserPollForms(string email, bool filled);
        
        Task<Response<List<GetPollFormDto>>> GetPollsterPollForms(string email, bool archived);

        Task<Response<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, string email);
        
        Task<Response<GetPollFormDto>> RemovePollForm(int pollId, string email);

        Task<Response<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, string email);
    }
}