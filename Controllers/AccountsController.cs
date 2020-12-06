using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Microsoft.AspNetCore.Mvc;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.AccountDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Ankietyzator.Controllers
{
    [Authorize]
    [ApiController]
    [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IRegisterService _register;

        public AccountsController(IRegisterService register)
        {
            _register = register;
        }

        //===================== GET =======================//

        [HttpGet("get-claims")]
        public IActionResult GetClaims()
        {
            var claims = HttpContext.User.Claims.Select(c => c.Type + " " + c.Value).ToList();
            return Ok(claims);
        }
        
        [HttpGet("get-account")]
        public async Task<IActionResult> GetAccount()
        {
            var accountResponse = await _register.GetAccount(GetUserEmail());
            var response = new Response<Account>(accountResponse);
            if (accountResponse.Code == HttpStatusCode.NotFound) NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-account/{email}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSpecificAccount(string email)
        {
            var accountResponse = await _register.GetAccount(email);
            var response = new Response<Account>(accountResponse);
            if (accountResponse.Code == HttpStatusCode.NotFound) NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-accounts")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAccounts()
        {
            var accountsResponse = await _register.GetAccounts();
            return Ok(new Response<List<Account>>(accountsResponse));
        }

        //===================== UPDATE =======================//

        [HttpPut("update-other-account")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateOtherAccount(UpdateAccountDto updateAccountDto)
        {
            ServiceResponse<Account> accountResponse = await _register.UpdateMyAccount(updateAccountDto, HttpContext);
            var response = new Response<Account>(accountResponse);
            return accountResponse.Code switch
            {
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.NotFound => NotFound(response),
                _ => Ok(response)
            };
        }
        
        [HttpPut("update-my-account")]
        public async Task<IActionResult> UpdateMyAccount(UpdateAccountDto updateAccountDto)
        {
            updateAccountDto.EMail = GetUserEmail();
            ServiceResponse<Account> accountResponse = await _register.UpdateMyAccount(updateAccountDto, HttpContext);
            var response = new Response<Account>(accountResponse);
            return accountResponse.Code switch
            {
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.NotFound => NotFound(response),
                _ => Ok(response)
            };
        }

        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}