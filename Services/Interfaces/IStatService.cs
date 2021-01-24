using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.QuestionDTOs;
using Ankietyzator.Models.DTO.StatsDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IStatService// : IDbContextService
    {
        Task<ServiceResponse<GetPollStatsDto>> GetPollStat(int pollId);

        Task<ServiceResponse<List<GetPollStatsDto>>> GetPollsStats(string pollsterEmail);

        Task<ServiceResponse<List<GetQuestionStatsDto>>> GetQuestionsStats(int pollId);

        Task<ServiceResponse<GetPollStatsDto>> CreatePollStats(int pollForm);

        Task<ServiceResponse<List<GetQuestionStatsDto>>> CreateQuestionsStats(List<GetQuestionDto> questions);

        /*Task<Response<object>> RemovePollStats(int pollId);

        Task<Response<object>> RemoveQuestionsStats(int pollId);*/
    }
}