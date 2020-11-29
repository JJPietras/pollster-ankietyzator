using Ankietyzator.Models.DataModel.PollModel;

namespace Ankietyzator.Models.DTO.QuestionDTOs
{
    public class GetQuestionDto
    {
        public int QuestionId { get; set; }
        
        public uint Position { get; set; }
        
        public string Title { get; set; }
        
        public string Options { get; set; }
        
        public bool AllowEmpty { get; set; }
        
        public ushort MaxLength { get; set; }
        
        public QuestionType Type { get; set; }
    }
}