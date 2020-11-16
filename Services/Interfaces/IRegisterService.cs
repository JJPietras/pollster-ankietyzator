using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO;
using Ankietyzator.Models.DTO.Account;

namespace Ankietyzator.Services.Interfaces
{
    public interface IRegisterService
    {
        AnkietyzatorDBContext Context { set; }
        Task<Response<GetAccountDto>> RegisterAccount(AddAccountDto addAccountDto);
        Task<Response<GetAccountDto>> UpdateAccount(UpdateAccountDto updateAccountDto);
        Task<Response<List<GetAccountDto>>> GetAccounts();
    }
}