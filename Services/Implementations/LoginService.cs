using System.Linq;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;
using Ankietyzator.Models.DTO.Login;
using Ankietyzator.Services.Interfaces;
using AutoMapper;

namespace Ankietyzator.Services.Implementations
{
    public class LoginService : ILoginService
    {
        private const string NoLoginDataStr = "No username nor email was provided";
        private const string WrongCredentialsStr = "Username/Login or password is incorrect";
        private const string SuccessfulLoginStr = "Login successful";

        public AnkietyzatorDBContext Context { get; set; }
        private readonly IMapper _mapper;

        public LoginService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Response<GetAccountDto> LoginToAccount(LoginDto loginDto)
        {
            var response = new Response<GetAccountDto>();
            bool hasLogin = loginDto.UserName != null;
            bool hasEMail = loginDto.EMail != null;

            if (!(hasLogin || hasEMail)) return Failure(response, NoLoginDataStr);

            var potentialAccounts = hasLogin
                ? Context.Accounts.Where(a => a.UserName == loginDto.UserName)
                : Context.Accounts.Where(a => a.EMail == loginDto.EMail);

            Account account =
                potentialAccounts.FirstOrDefault(a => a.PasswordHash == Account.GetHash(loginDto.Password));
            return account == null
                ? Failure(response, WrongCredentialsStr)
                : Success(response, _mapper.Map<GetAccountDto>(account), SuccessfulLoginStr);
        }

        //======================================= RESULTS =======================================//

        private static Response<T> Failure<T>(Response<T> accountResponse, string message)
        {
            accountResponse.Message = message;
            accountResponse.Success = false;
            return accountResponse;
        }

        private static Response<T> Success<T>(Response<T> accountResponse, T account, string message)
        {
            accountResponse.Data = account;
            accountResponse.Message = message;
            return accountResponse;
        }
    }
}