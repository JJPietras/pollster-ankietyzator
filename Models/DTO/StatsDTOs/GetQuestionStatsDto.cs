namespace Ankietyzator.Models.DTO.StatsDTOs
{
    public class GetQuestionStatsDto
    {
        public int QuestionId { get; set; }
        
        public int Position { get; set; }
        
        public string Title { get; set; }
        
        public string Options { get; set; }
        
        public bool AllowEmpty { get; set; }
        
        public short MaxLength { get; set; }
        
        public int Type { get; set; }

        public string AnswerCounts { get; set; }
    }
}