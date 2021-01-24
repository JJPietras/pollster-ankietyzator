using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.KeyDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IKeyService// : IDbContextService
    {
        Task<ServiceResponse<List<UpgradeKey>>> GetUpgradeKeys();
        
        Task<ServiceResponse<UpgradeKey>> GetUpgradeKey(string key);
        
        Task<ServiceResponse<UpgradeKey>> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey);
        
        Task<ServiceResponse<UpgradeKey>> RemoveUpgradeKey(string key);

        Task<ServiceResponse<UpgradeKey>> AddUpgradeKey(UpgradeKey upgradeKey);
        
    }
}