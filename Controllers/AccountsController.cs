using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Microsoft.AspNetCore.Mvc;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Ankietyzator.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AnkietyzatorDBContext _context;

        private readonly IRegisterService _register;

        public AccountsController(AnkietyzatorDBContext context, IRegisterService register)
        {
            _context = context;
            _register = register;
            _register.Context = context;
        }

        //===================== GET =======================//

        [HttpGet("account")]
        public async Task<IActionResult> GetAccount()
        {
            string mail = HttpContext.User.Claims.ToArray()[2].Value;
            Response<Account> accountResponse = await _register.GetAccount(mail);
            if (accountResponse.Data != null) return Ok(accountResponse);
            return NotFound(accountResponse);
        }

        //TODO: test it later
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            string mail = HttpContext.User.Claims.ToArray()[2].Value;
            Response<Account> accountResponse = await _register.GetAccount(mail);
            if (accountResponse.Data != null)
            {
                UserType userType = accountResponse.Data.UserType;
                Response<List<Account>> accountsResponse = await _register.GetAccounts(userType);
                if (accountsResponse.Data != null) return Ok(accountsResponse);
                return Unauthorized(accountsResponse);
            }
            
            return NotFound(accountResponse);
        }

        //TODO: change
        // GET: api/Accounts/5
        [HttpGet("accounts/{id}")]
        public async Task<ActionResult<Account>> GetAccount(int? id)
        {
            Account getAccountDto = await _context.Accounts.FindAsync(id);
            if (getAccountDto == null) return NotFound();
            return getAccountDto;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            Response<Account> accountResponse = await _register.UpdateAccount(updateAccountDto);
            if (accountResponse.Data == null) return Conflict(accountResponse);
            return Ok(accountResponse); 
        }
        /* DELETE: api/Accounts/5
        [HttpDelete("account/{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(int? id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return account;
        }*/
    }
}