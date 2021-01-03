namespace Ankietyzator.Models.DTO.StatsDTOs
{
    public class GetPollStatsDto
    {
        public int PollId { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public int Completions { get; set; }

        public double Percentage { get; set; }
    }
}