using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IStatService// : IDbContextService
    {
        Task<Response<PollStat>> GetPollStat(int pollId);

        Task<Response<List<PollStat>>> GetPollsStats(string pollsterEmail);

        Task<Response<List<QuestionStat>>> GetQuestionsStats(int pollId);

        Task<Response<PollStat>> CreatePollStats(int pollForm);

        Task<Response<List<QuestionStat>>> CreateQuestionsStats(List<GetQuestionDto> questions);

        /*Task<Response<object>> RemovePollStats(int pollId);

        Task<Response<object>> RemoveQuestionsStats(int pollId);*/
    }
}