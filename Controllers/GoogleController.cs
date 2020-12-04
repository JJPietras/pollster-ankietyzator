using System;
using System.Collections.Generic;
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
            AuthenticationProperties properties = new AuthenticationProperties
                {RedirectUri = Url.Action("GoogleResponse")};
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            const string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            AuthenticateResult result = await HttpContext.AuthenticateAsync(cookieScheme);
            
            (bool registered, Account account) = await Register(result.Principal.Claims.ToArray());
            string role = await AddAccountTypeRole(result, account);

            string message = registered ? "User signed-up: " : "Registered new user: ";
            Console.WriteLine(message + account.EMail + " " + account.Name + " role: " + role);
            return Redirect("~/");
        }

        [Route("google-logout")]
        public async Task<RedirectResult> Logout()
        {
            await HttpContext.SignOutAsync();
            string baseString = "https://www.google.com/accounts/Logout?continue=";
            string appEngine = "https://appengine.google.com/_ah/logout?continue="; 
            return Redirect(baseString + appEngine + "https://localhost:5001");
        }
        
        //===================================== HELPER METHODS =====================================//

        private async Task<(bool, Account)> Register(IReadOnlyList<Claim> resultClaims)
        {
            bool registered = false;
            string email = resultClaims[EMail].Value;
            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            if (account == null)
            {
                Account newAccount = new Account
                {
                    EMail = email,
                    Name = resultClaims[Name].Value,
                    Tags = "",
                    UserType = UserType.User
                };
                await _context.Accounts.AddAsync(newAccount);
                await _context.SaveChangesAsync();
            }
            else registered = true;

            return (registered, account);
        }

        private async Task<string> AddAccountTypeRole(AuthenticateResult result, Account account)
        {
            var claim = new ClaimsIdentity();
            string role = account.UserType.GetRole();
            claim.AddClaim(new Claim(ClaimTypes.Role, role));
            result.Principal.AddIdentity(claim);
            await HttpContext.SignInAsync(result.Principal, result.Properties);
            return role;
        }
    }
}