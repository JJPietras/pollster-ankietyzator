using Ankietyzator.Models.DataModel.AccountModel;

namespace Ankietyzator.Models.DTO.KeyDTOs
{
    public class UpdateUpgradeKeyDto
    {
        public string Key { get; set; }
        
        public string EMail { get; set; }
        
        public UserType UserType { get; set; }
    }
}