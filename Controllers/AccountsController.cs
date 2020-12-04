using System;
using System.Linq;
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

        [HttpGet("get-account")]
        public async Task<IActionResult> GetAccount()
        {
            Response<Account> accountResponse = await _register.GetAccount(GetUserEmail());
            if (accountResponse.Data != null) return Ok(accountResponse);
            return NotFound(accountResponse);
        }
        
        [HttpGet("get-account/{email}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSpecificAccount(string email)
        {
            Response<Account> accountResponse = await _register.GetAccount(email);
            if (accountResponse.Data != null) return Ok(accountResponse);
            return NotFound(accountResponse);
        }

        [HttpGet("get-accounts")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAccounts()
        {
            var response = await _register.GetAccounts();
            if (response.Data != null) return Ok(response);
            return Unauthorized(response);
        }

        //===================== UPDATE =======================//

        [HttpPut("update-account")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            Console.WriteLine(HttpContext.User.Claims.LastOrDefault()?.Value);
            if (updateAccountDto.EMail != GetUserEmail()) return Unauthorized(updateAccountDto);
            Response<Account> accountResponse = await _register.UpdateAccount(updateAccountDto, HttpContext);
            if (accountResponse.Data == null) return Conflict(accountResponse);
            Console.WriteLine(HttpContext.User.Claims.LastOrDefault()?.Value);
            return Ok(accountResponse);
        }


        private string GetUserEmail() => HttpContext.User.Claims.ToArray()[2].Value;
    }
}