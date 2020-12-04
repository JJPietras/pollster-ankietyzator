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
        private const string InvalidIndexStr = "Provided no or invalid account Email";
        private const string SuccessfulUpdateStr = "Account updated successfuly";

        private const string NoAccountStr = "There is no registered account with this email";
        private const string AccountFoundStr = "Account fetched successfuly";

        private const string InvalidKeyStr = "Provided polster key is invalid";
        private const string GetAccountsStr = "Successfuly queried registrations";

        private readonly AnkietyzatorDbContext _context;

        private readonly IKeyService _keyService;
        private readonly IMapper _mapper;

        public RegisterService(AnkietyzatorDbContext context, IMapper mapper, IKeyService keyService)
        {
            _keyService = keyService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<Response<Account>> GetAccount(string email)
        {
            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            var response = new Response<Account>();
            return account == null ? response.Failure(NoAccountStr) : response.Success(account, AccountFoundStr);
        }
        
        public async Task<Response<List<Account>>> GetAccounts()
        {
            var response = new Response<List<Account>>();
            var accounts = await _context.Accounts.ToListAsync();
            return response.Success(accounts, GetAccountsStr);
        }

        public async Task<Response<Account>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var response = new Response<Account>();
            (bool changedTags, bool changedKey) = GetConditions(updateAccountDto); 

            int changes = new[]{changedTags, changedKey}.Count(b => b);
            if (changes < 1) return response.Failure(NoUpdatesStr);
            if (changes > 1) return response.Failure(ManyUpdatesStr);

            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == updateAccountDto.EMail);
            if (account == null) return response.Failure(InvalidIndexStr);

            var keyResponse = await _keyService.GetPollsterKey(updateAccountDto.PollsterKey);
            if (changedKey && keyResponse.Data == null) return response.Failure(keyResponse.Message);
            var key = keyResponse.Data;
            
            if (InvalidPollsterKey(updateAccountDto, key, account.EMail)) return response.Failure(InvalidKeyStr);
            
            MakeAccountChanges(account, updateAccountDto);
            await _context.SaveChangesAsync();
            
            return response.Success(_mapper.Map<Account>(account), SuccessfulUpdateStr);
        }

        //======================================= UPDATE =======================================//

        private static bool InvalidPollsterKey(UpdateAccountDto createAccountDto, UpgradeKey key, string eMail)
        {
            string dtoKey = createAccountDto.PollsterKey;
            if (dtoKey == null) return false;
            return key == null || key.EMail != eMail && key.EMail.Length > 0;
        }

        private static (bool, bool) GetConditions(UpdateAccountDto dto) => (dto.Tags != null, dto.PollsterKey != null);

        private static void MakeAccountChanges(Account account, UpdateAccountDto dto)
        {
            if (dto.Tags != null) account.Tags = dto.Tags;
            if (dto.PollsterKey != null) account.UserType = UserType.Pollster;
        }
    }
}