using System.Collections.Generic;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Models.DTO.PollDTOs
{
    public class GetPollFormDto
    {
        public int? PollId { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Tags { get; set; }
        
        public string Emails { get; set; }

        public bool NonAnonymous { get; set; }
        
        public bool Archived { get; set; }
        
        public List<GetQuestionDto> Questions { get; set; } = new List<GetQuestionDto>();
    }
}