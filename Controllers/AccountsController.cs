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

        public AccountsController(AnkietyzatorDbContext context, IRegisterService register)
        {
            _register = register;
            _register.Context = context;
        }

        //===================== GET =======================//

        [HttpGet("get-account")]
        public async Task<IActionResult> GetAccount(string mail)
        {
            Response<Account> accountResponse = await _register.GetAccount(mail);
            if (accountResponse.Data != null) return Ok(accountResponse);
            return NotFound(accountResponse);
        }

        //TODO: test it later
        [HttpGet("get-accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            //TODO: change
            string mail = GetUserEmail();
            Response<Account> accountResponse = await _register.GetAccount(mail);
            if (accountResponse.Data == null) return NotFound(accountResponse);
            UserType userType = accountResponse.Data.UserType;
            
            //TODO: authorisation
            var response = await _register.GetAccounts(userType);
            if (response.Data != null) return Ok(response);
            return Unauthorized(response);

        }
        
        //===================== UPDATE =======================//

        [HttpPut("update-account")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            //TODO: add authorisation
            Response<Account> accountResponse = await _register.UpdateAccount(updateAccountDto);
            if (accountResponse.Data == null) return Conflict(accountResponse);
            return Ok(accountResponse); 
        }
        
        //TODO: create CreateAccount

        private string GetUserEmail() => HttpContext.User.Claims.ToArray()[2].Value;
    }
}