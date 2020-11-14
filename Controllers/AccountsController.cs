using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.ViewModel;

namespace Ankietyzator.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AnkietyzatorDBContext _context;

        public AccountsController(AnkietyzatorDBContext context)
        {
            _context = context;
        }

        //===================== GET =======================//


        // GET: api/Accounts
        [HttpGet("accounts")]
        public IEnumerable<AccountView> GetAccounts() => _context.Accounts
            .Select(a => new AccountView
            {
                Id = a.Id,
                UserName = a.UserName.TrimEnd(),
                EMail = a.EMail.TrimEnd(),
                UserType = a.UserType
            }).ToList();


        //TESTER
        [HttpGet("inserter")]
        public async Task<ActionResult> AddData()
        {
            await _context.Accounts.AddAsync(new Account
            {
                EMail = "mymail@mail.com", PasswordHash = new byte[] {0x24}, UserName = "UZAR",
                UserType = UserType.Pollster
            });
            await _context.SaveChangesAsync();
            Account account = (from d in _context.Accounts select d).FirstOrDefault();

            Console.WriteLine(
                "UserName: {0} UserType: {1}",
                account.UserName,
                account.UserType);
            return NoContent();
        }

        // GET: api/Accounts/5
        [HttpGet("accounts/{id}")]
        public async Task<ActionResult<Account>> GetAccount(int? id)
        {
            Account account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();
            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("accounts/{id}")]
        public async Task<IActionResult> PutAccount(int? id, Account account)
        {
            if (id != account.Id) return BadRequest();

            _context.Entry(account).State = EntityState.Modified;

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

        [HttpPut("accounts")]
        public async Task<ActionResult<AccountView>> PutAccount([FromBody] AccountRequest accountRequest)
        {
            Account account = _context.Accounts.FirstOrDefault(
                a => a.UserName == accountRequest.UserName && a.PasswordHash == accountRequest.GetHash());
            
            if (account != null) return new ActionResult<AccountView>(new AccountView
            {
                Id = account.Id,
                EMail = account.EMail.TrimEnd(),
                UserName = account.UserName.TrimEnd(), 
                UserType = account.UserType
            });
            
            return NotFound();
        }

        // POST: api/Accounts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("accounts")]
        public async Task<ActionResult<Account>> PostAccount(AccountRequest accountRequest)
        {
            Account newAccount = new Account
            {
                EMail = accountRequest.EMail.TrimEnd(),
                UserName = accountRequest.UserName.TrimEnd(),
                PasswordHash = accountRequest.GetHash(),
                UserType = UserType.User
            };

            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new {id = newAccount.Id}, accountRequest);
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