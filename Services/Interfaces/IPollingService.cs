using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.PollDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IPollingService : IDbContextService
    {
        Task<Response<GetPollFormDto>> GetPollForm(int pollId);

        Task<Response<List<GetPollFormDto>>> GetPollForms(int pollsterId);

        Task<Response<List<GetPollFormDto>>> GetArchivedPollForms(int pollsterId);
        
        Task<Response<List<GetPollFormDto>>> GetNotArchivedPollForms(int pollsterId);

        Task<Response<object>> RemovePollForm(int pollId);

        Task<Response<GetPollFormDto>> UpdatePollForm(UpdatePollFormDto pollForm, int accountId);

        Task<Response<GetPollFormDto>> CreatePollForm(CreatePollFormDto pollForm, int accountId);
    }
}