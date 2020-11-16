namespace Ankietyzator.Models.DTO.Account
{
    public class AddAccountDto
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string PollsterKey { get; set; }
    }
}