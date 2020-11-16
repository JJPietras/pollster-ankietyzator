using Ankietyzator.Models;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;
using Ankietyzator.Models.DTO.Login;

namespace Ankietyzator.Services.Interfaces
{
    public interface ILoginService
    {
        AnkietyzatorDBContext Context { set; }
        Response<GetAccountDto> LoginToAccount(LoginDto loginDto);
    }
}