using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.KeyDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IKeyService// : IDbContextService
    {
        Task<Response<List<UpgradeKey>>> GetUpgradeKeys();
        
        Task<Response<UpgradeKey>> GetUpgradeKey(string key);
        
        Task<Response<UpgradeKey>> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey);
        
        Task<Response<UpgradeKey>> RemoveUpgradeKey(string key);

        Task<Response<UpgradeKey>> AddUpgradeKey(UpgradeKey upgradeKey);
        
    }
}