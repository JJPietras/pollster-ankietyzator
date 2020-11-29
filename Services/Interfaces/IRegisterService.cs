using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.AccountDTOs;

namespace Ankietyzator.Services.Interfaces
{
    public interface IRegisterService : IDbContextService
    {
        Task<Response<List<Account>>> GetAccounts(UserType userType);
        Task<Response<Account>> GetAccount(string email);
        Task<Response<Account>> UpdateAccount(UpdateAccountDto updateAccountDto);
    }
}