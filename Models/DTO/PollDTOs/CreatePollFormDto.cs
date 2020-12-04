using System.Collections.Generic;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Models.DTO.PollDTOs
{
    public class CreatePollFormDto
    {
        public string Tags { get; set; }
        
        public string Emails { get; set; }
        
        public bool NonAnonymous { get; set; }
        
        public List<CreateQuestionDto> Questions { get; set; }
    }
}