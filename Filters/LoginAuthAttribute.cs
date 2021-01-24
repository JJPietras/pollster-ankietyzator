using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ankietyzator.Filters
{
    public class LoginAuthAttribute : AuthorizeAttribute
    {
        //public override void OnAuthorization(HttpActionContext )
    }
}