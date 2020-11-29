namespace Ankietyzator.Models.DTO.AccountDTOs
{
    public class UpdateAccountDto
    {
        public int? Id { get; set; }
        public string Tags { get; set; }
        public string PollsterKey { get; set; }
    }
}