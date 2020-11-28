using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel;
using Ankietyzator.Models.DTO.Account;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class RegisterService : IRegisterService
    {
        /*private const string MissingRegistrationDataStr = "Account registered with this email exists";
        private const string EmailExistsStr = "Account registered with this email exists";
        private const string UsernameExistsStr = "This username is already used";
        private const string PollsterKeyNotExistsStr = "Invalid pollster key";
        private const string SuccessfulRegistration = "New account created successfully";*/
        
        private const string NoUpdatesStr = "No updates were passed";
        private const string ManyUpdatesStr = "There can only be one update per request";
        private const string InvalidIndexStr = "Provided no or invalid account index";
        private const string SuccessfulUpdateStr = "Account updated successfully";

        private const string NoAccountStr = "There is no registered account with this email";
        private const string AccountFoundStr = "Account fetched successfully";

        private const string UserNotAdminStr = "Could not fetch users data. User is not Admin";
        private const string GetAccountsStr = "Successfully queried registrations";

        public AnkietyzatorDBContext Context { get; set; }
        
        private readonly IMapper _mapper;

        public RegisterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Response<Account>> GetAccount(string email)
        {
            Account account = await Context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            var response = new Response<Account>();
            if (account == null) return Failure(response, NoAccountStr);
            return Success(response, account, AccountFoundStr);
        }
        
        public async Task<Response<List<Account>>> GetAccounts(UserType userType)
        {
            var response = new Response<List<Account>>();
            if (userType != UserType.Admin) return Failure(response, UserNotAdminStr);
            var accounts = await Context.Accounts.ToListAsync();
            return Success(response, accounts, GetAccountsStr);
        }

        public async Task<Response<Account>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var response = new Response<Account>();
            (bool changedTags, bool changedPollsterKey) = GetConditions(updateAccountDto); 

            int changes = new[]{changedTags, changedPollsterKey}.Count(b => b);
            if (changes < 1) return Failure(response, NoUpdatesStr);
            if (changes > 1) return Failure(response, ManyUpdatesStr);

            Account account = await Context.Accounts.FirstOrDefaultAsync(a => a.Id == updateAccountDto.Id);
            if (account == null) return Failure(response, InvalidIndexStr);
            MakeAccountChanges(account, updateAccountDto);

            await Context.SaveChangesAsync();
            
            return Success(response, _mapper.Map<Account>(account), SuccessfulUpdateStr);
        }


        //======================================= ADD =======================================//

        //TODO: implement
        /*private static bool PollsterKeyNotExists(AddAccountDto addAccountDto) =>
            addAccountDto.PollsterKey != null /*&& TODO#3#;*/

        /*private Account MapAddAccountDtoOnoAccount(AddAccountDto addAccountDto)
        {
            Account newAccount = _mapper.Map<Account>(addAccountDto);
            newAccount.PasswordHash = Account.GetHash(addAccountDto.Password);
            newAccount.UserType = UserType.User;
            return newAccount;
        }#2# #1#*/
        
        //======================================= UPDATE =======================================//

        private static (bool, bool) GetConditions(UpdateAccountDto dto) => (dto.Tags != null, dto.PollsterKey != null);

        private void MakeAccountChanges(Account account, UpdateAccountDto dto)
        {
            if (dto.Tags != null) account.Tags = dto.Tags;
            if (dto.PollsterKey != null) account.UserType = UserType.Pollster;
        }
        
        //======================================= RESULTS =======================================//
        
        private static Response<T> Failure<T>(Response<T> accountResponse, string message)
        {
            accountResponse.Message = message;
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