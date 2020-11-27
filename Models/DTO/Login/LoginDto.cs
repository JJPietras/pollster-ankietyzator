using Microsoft.AspNetCore.Authentication;

namespace Ankietyzator.Models.DTO.Login
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }
        
        public AuthenticationScheme ExternalLogin { get; set; }
    }
}