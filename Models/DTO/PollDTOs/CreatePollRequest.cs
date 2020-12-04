namespace Ankietyzator.Models.DTO.PollDTOs
{
    public class CreatePollRequest
    {
        public int AccountId { get; set; }
        public CreatePollFormDto PollForm { get; set; }
        
    }
}