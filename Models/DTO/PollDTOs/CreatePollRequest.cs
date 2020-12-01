using System.Collections.Generic;
using Ankietyzator.Models.DTO.QuestionDTOs;

namespace Ankietyzator.Models.DTO.PollDTOs
{
    public class CreatePollRequest
    {
        public int accountId { get; set; }
        public CreatePollFormDto pollForm { get; set; }
        
    }
}