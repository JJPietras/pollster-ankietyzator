using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IStatService// : IDbContextService
    {
        Task<ServiceResponse<PollStat>> GetPollStat(int pollId);

        Task<ServiceResponse<List<PollStat>>> GetPollsStats(string pollsterEmail);

        Task<ServiceResponse<List<QuestionStat>>> GetQuestionsStats(int pollId);

        Task<ServiceResponse<PollStat>> CreatePollStats(int pollForm);

        Task<ServiceResponse<List<QuestionStat>>> CreateQuestionsStats(List<GetQuestionDto> questions);

        /*Task<Response<object>> RemovePollStats(int pollId);

        Task<Response<object>> RemoveQuestionsStats(int pollId);*/
    }
}