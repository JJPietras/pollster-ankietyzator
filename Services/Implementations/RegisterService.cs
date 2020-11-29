using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.AccountDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class RegisterService : IRegisterService
    {
        private const string NoUpdatesStr = "No updates were passed";
        private const string ManyUpdatesStr = "There can only be one update per request";
        private const string InvalidIndexStr = "Provided no or invalid account index";
        private const string SuccessfulUpdateStr = "Account updated successfuly";

        private const string NoAccountStr = "There is no registered account with this email";
        private const string AccountFoundStr = "Account fetched successfuly";

        private const string UserNotAdminStr = "Could not fetch users data. User is not Admin";
        private const string GetAccountsStr = "Successfuly queried registrations";

        public AnkietyzatorDbContext Context { get; set; }
        
        private readonly IMapper _mapper;

        public RegisterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Response<Account>> GetAccount(string email)
        {
            Account account = await Context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            var response = new Response<Account>();
            if (account == null) return response.Failure(NoAccountStr);
            return response.Success(account, AccountFoundStr);
        }
        
        public async Task<Response<List<Account>>> GetAccounts(UserType userType)
        {
            var response = new Response<List<Account>>();
            if (userType != UserType.Admin) return response.Failure(UserNotAdminStr);
            var accounts = await Context.Accounts.ToListAsync();
            return response.Success(accounts, GetAccountsStr);
        }

        public async Task<Response<Account>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var response = new Response<Account>();
            (bool changedTags, bool changedPollsterKey) = GetConditions(updateAccountDto); 

            int changes = new[]{changedTags, changedPollsterKey}.Count(b => b);
            if (changes < 1) return response.Failure(NoUpdatesStr);
            if (changes > 1) return response.Failure(ManyUpdatesStr);

            Account account = await Context.Accounts.FirstOrDefaultAsync(a => a.AccountId == updateAccountDto.Id);
            if (account == null) return response.Failure(InvalidIndexStr);
            MakeAccountChanges(account, updateAccountDto);

            await Context.SaveChangesAsync();
            
            return response.Success(_mapper.Map<Account>(account), SuccessfulUpdateStr);
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

        private static void MakeAccountChanges(Account account, UpdateAccountDto dto)
        {
            if (dto.Tags != null) account.Tags = dto.Tags;
            if (dto.PollsterKey != null) account.UserType = UserType.Pollster;
        }
    }
}