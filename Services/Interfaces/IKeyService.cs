using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.KeyDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IKeyService// : IDbContextService
    {
        Task<Response<List<UpgradeKey>>> GetPollsterKeys();
        
        Task<Response<UpgradeKey>> GetPollsterKey(string key);
        
        Task<Response<UpgradeKey>> UpdatePollsterKey(UpdateUpgradeKeyDto upgradeKey);
        
        Task<Response<UpgradeKey>> RemovePollsterKey(string key);

        Task<Response<UpgradeKey>> AddPollsterKey(UpgradeKey upgradeKey);
        
    }
}