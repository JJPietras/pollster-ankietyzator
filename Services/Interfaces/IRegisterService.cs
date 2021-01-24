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
        Task<ServiceResponse<List<Account>>> GetAccounts();
        Task<ServiceResponse<Account>> GetAccount(string email);
        Task<ServiceResponse<Account>> UpdateMyAccount(UpdateAccountDto updateAccountDto, HttpContext context);
        Task<ServiceResponse<Account>> UpdateOtherAccount(UpdateAccountDto updateAccountDto);
    }
}