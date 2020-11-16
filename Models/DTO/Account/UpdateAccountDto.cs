namespace Ankietyzator.Models.DTO.Account
{
    public class UpdateAccountDto
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        //TODO: new old passwords
        public string Password { get; set; }
        public string PollsterKey { get; set; }
    }
}