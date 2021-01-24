using System.Collections.Generic;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Models.DTO.PollDTOs
{
    public class UpdatePollFormDto
    {
        public int PreviousPollId { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Tags { get; set; }
        
        public string Emails { get; set; }
        
        public bool NonAnonymous { get; set; } = false;
        
        public List<CreateQuestionDto> Questions { get; set; }
    }
}