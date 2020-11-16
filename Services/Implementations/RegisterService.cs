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
        private const string MissingRegistrationDataStr = "Account registered with this email exists";
        private const string EmailExistsStr = "Account registered with this email exists";
        private const string UsernameExistsStr = "This username is already used";
        private const string PollsterKeyNotExistsStr = "Invalid pollster key";
        private const string SuccessfulRegistration = "New account created successfully";
        
        private const string NoUpdatesStr = "No updates were passed";
        private const string ManyUpdatesStr = "There can only be one update per request";
        private const string InvalidIndexStr = "Provided no or invalid account index";
        private const string SuccessfulUpdateStr = "Account updated successfully";

        private const string GetAccountsStr = "Successfully queried registrations";

        public AnkietyzatorDBContext Context { get; set; }
        
        private readonly IMapper _mapper;

        public RegisterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Response<GetAccountDto>> RegisterAccount(AddAccountDto addAccountDto)
        {
            var response = new Response<GetAccountDto>();
            if (MissingRegistrationData(addAccountDto)) return Failure(response, MissingRegistrationDataStr);
            if (PollsterKeyNotExists(addAccountDto)) return Failure(response, PollsterKeyNotExistsStr);
            if (UsernameExists(addAccountDto)) return Failure(response, UsernameExistsStr);
            if (EmailExists(addAccountDto)) return Failure(response, EmailExistsStr);

            Account newAccount = MapAddAccountDtoOnoAccount(addAccountDto);

            await Context.Accounts.AddAsync(newAccount);
            await Context.SaveChangesAsync();

            return Success(response, _mapper.Map<GetAccountDto>(newAccount), SuccessfulRegistration);
        }

        public async Task<Response<GetAccountDto>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var response = new Response<GetAccountDto>();
            (bool changedPassword, bool changedPollsterKey, bool changedUserName) = GetConditions(updateAccountDto); 

            int changes = new[]{changedPassword, changedPollsterKey, changedUserName}.Count(b => b);
            if (changes < 1) return Failure(response, NoUpdatesStr);
            if (changes > 1) return Failure(response, ManyUpdatesStr);

            Account account = await Context.Accounts.FirstOrDefaultAsync(a => a.Id == updateAccountDto.Id);
            if (account == null) return Failure(response, InvalidIndexStr);
            MakeAccountChanges(account, changedUserName, changedPassword, changedPollsterKey, updateAccountDto);

            await Context.SaveChangesAsync();
            
            return Success(response, _mapper.Map<GetAccountDto>(account), SuccessfulUpdateStr);
        }

        //TODO: remove if not needed
        public async Task<Response<List<GetAccountDto>>> GetAccounts()
        {
            var accounts = await Context.Accounts.ToListAsync();
            var getAccountDtos = accounts.Select(a => _mapper.Map<GetAccountDto>(a)).ToList();
            var response = new Response<List<GetAccountDto>> {Data = getAccountDtos, Message = GetAccountsStr};
            return response;
        }

        //======================================= ADD =======================================//

        private static bool MissingRegistrationData(AddAccountDto addAccountDto) =>
            addAccountDto.Password == null || addAccountDto.EMail == null || addAccountDto.UserName == null;
        private bool EmailExists(AddAccountDto addAccountDto) =>
            Context.Accounts.FirstOrDefault(a => a.EMail == addAccountDto.EMail) != null;

        private bool UsernameExists(AddAccountDto addAccountDto) =>
            Context.Accounts.FirstOrDefault(a => a.UserName == addAccountDto.UserName) != null;

        //TODO: add pollster keys to the database
        private static bool PollsterKeyNotExists(AddAccountDto addAccountDto) =>
            addAccountDto.PollsterKey != null /*&& TODO*/;

        private Account MapAddAccountDtoOnoAccount(AddAccountDto addAccountDto)
        {
            Account newAccount = _mapper.Map<Account>(addAccountDto);
            newAccount.PasswordHash = Account.GetHash(addAccountDto.Password);
            newAccount.UserType = UserType.User;
            return newAccount;
        }
        
        //======================================= UPDATE =======================================//

        private (bool, bool, bool) GetConditions(UpdateAccountDto dto) =>
            (dto.Password != null, dto.PollsterKey != null, dto.UserName != null);

        private void MakeAccountChanges(Account account, bool name, bool password, bool key, UpdateAccountDto dto)
        {
            if (name) account.UserName = dto.UserName;
            if (password) account.PasswordHash = Account.GetHash(dto.Password);
            if (key /*&& TODO*/) account.UserType = UserType.Pollster;
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