using Ankietyzator.Models.DataModel.PollModel;
using Ankietyzator.Models.DTO.AnswerDTOs;
using Ankietyzator.Models.DTO.PollDTOs;
using Ankietyzator.Models.DTO.QuestionDTOs;
using AutoMapper;

namespace Ankietyzator
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Answer, GetAnswerDto>();
            CreateMap<CreateAnswerDto, Answer>();
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<Question, GetQuestionDto>();
            CreateMap<PollForm, GetPollFormDto>();
            CreateMap<PollForm, CreatePollFormDto>();
            CreateMap<CreatePollFormDto, PollForm>();
            CreateMap<UpdatePollFormDto, CreatePollFormDto>();
        }
    }
}