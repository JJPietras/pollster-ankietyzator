using System.Collections.Generic;

namespace Ankietyzator.Models.DTO.AnswerDTOs
{
    public class GetDetailedAnswerDto
    {
        public int? AccountId { get; set; }
        
        public string EMail { get; set; }

        public string Name { get; set; }

        public List<GetAnswerDto> Answers { get; set; }
    }
}