using Ankietyzator.Models.DataModel;

namespace Ankietyzator.Models.ViewModel
{
    public class AccountView
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public UserType UserType { get; set; }
    }
}