namespace Ankietyzator.Models.DTO.AnswerDTOs
{
    public class CreateAnswerDto
    {
        public int? QuestionId { get; set; }
        
        public string Content { get; set; }
    }
}