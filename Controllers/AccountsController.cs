using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;
using Ankietyzator.Models.DTO.Login;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Ankietyzator.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    //[LoginAuth]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AnkietyzatorDBContext _context;

        private readonly IRegisterService _register;
        private readonly ILoginService _login;

        public AccountsController(AnkietyzatorDBContext context, IRegisterService register, ILoginService login)
        {
            _context = context;
            _register = register;
            _login = login;
            _register.Context = context;
            _login.Context = context;
        }

        //===================== GET =======================//

        // GET: api/Accounts
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            return Ok(await _register.GetAccounts());
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
        
        [HttpPut("accounts/{id}")]
        public async Task<IActionResult> PutAccount(int? id, Account getAccountDto)
        {
            if (id != getAccountDto.Id) return BadRequest();

            _context.Entry(getAccountDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            Response<GetAccountDto> accountResponse = await _register.UpdateAccount(updateAccountDto);
            if (!accountResponse.Success) return Conflict(accountResponse);
            return CreatedAtAction("UpdateAccount", accountResponse); 
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount(AddAccountDto addAccountDto)
        {
            Response<GetAccountDto> accountResponse = await _register.RegisterAccount(addAccountDto);
            if (!accountResponse.Success) return Conflict(accountResponse);
            return CreatedAtAction("RegisterAccount", accountResponse); 
        }
        
        [HttpPost("login")]
        public IActionResult LoginToAccount(LoginDto loginDto)
        {
            Response<GetAccountDto> accountResponse = _login.LoginToAccount(loginDto);
            if (!accountResponse.Success) return Conflict(accountResponse);
            return CreatedAtAction("LoginToAccount", accountResponse); 
        }

        // DELETE: api/Accounts/5
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
        }

        private bool AccountExists(int? id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}