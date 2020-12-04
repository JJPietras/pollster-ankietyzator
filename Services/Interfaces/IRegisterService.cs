using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.AccountDTOs;
using Microsoft.AspNetCore.Http;

namespace Ankietyzator.Services.Interfaces
{
    public interface IRegisterService// : IDbContextService
    {
        Task<Response<List<Account>>> GetAccounts();
        Task<Response<Account>> GetAccount(string email);
        Task<Response<Account>> UpdateMyAccount(UpdateAccountDto updateAccountDto, HttpContext context);
        Task<Response<Account>> UpdateOtherAccount(UpdateAccountDto updateAccountDto);
    }
}