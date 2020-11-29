using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Controllers
{
    [ApiController]
    [Route("google")]
    public class GoogleController : Controller
    {
        private const int Id = 0;
        private const int Name = 1;
        private const int EMail = 2;

        private readonly AnkietyzatorDbContext _context;

        public GoogleController(AnkietyzatorDbContext context) => _context = context;

        [AllowAnonymous]
        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            //signin-google
            AuthenticationProperties properties = new AuthenticationProperties
                {RedirectUri = Url.Action("GoogleResponse")};
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        //TODO: move part to RegisterService
        [AllowAnonymous]
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            const string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            AuthenticateResult result = await HttpContext.AuthenticateAsync(cookieScheme);

            Claim[] claims = result.Principal.Claims.ToArray();
            string email = claims[EMail].Value;
            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null)
            {
                Account newAccount = new Account
                {
                    EMail = email,
                    Name = claims[Name].Value,
                    Tags = "",
                    UserType = UserType.User
                };
                await _context.Accounts.AddAsync(newAccount);
                await _context.SaveChangesAsync();
                Console.WriteLine("Registered new user: " + newAccount.EMail + " " + newAccount.Name);
                return Redirect("/user-login");
            }

            Console.WriteLine("User signed-up: " + account.EMail + " " + account.Name);
            return Redirect("~/");
        }

        [Route("google-logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/user-login");
        }
    }
}