using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;

namespace Ankietyzator.Services.Interfaces
{
    public interface IRegisterService
    {
        AnkietyzatorDBContext Context { set; }
        Task<Response<List<Account>>> GetAccounts(UserType userType);
        Task<Response<Account>> GetAccount(string email);
        Task<Response<Account>> UpdateAccount(UpdateAccountDto updateAccountDto);
    }
}