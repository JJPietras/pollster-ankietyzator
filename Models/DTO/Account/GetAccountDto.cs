using Ankietyzator.Models.DataModel;

namespace Ankietyzator.Models.DTO.Account
{
    public class GetAccountDto
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public UserType UserType { get; set; }
    }
}