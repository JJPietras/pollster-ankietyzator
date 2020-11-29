using System.Threading.Tasks;
using Ankietyzator.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ankietyzator.Controllers
{
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly AnkietyzatorDbContext _context;
        
        public PollsController(AnkietyzatorDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("PollForms")]
        public async Task<IActionResult> GetPollForms()
        {
            /*string mail = HttpContext.User.Claims.ToArray()[2].Value;
            Response<Account> accountResponse = await _register.GetAccount(mail);
            if (accountResponse.Data != null) return Ok(accountResponse);
            return NotFound(accountResponse);*/
            return Ok(_context.PollForms);
        }
    }
}