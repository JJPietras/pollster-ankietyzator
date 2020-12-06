using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.AccountDTOs;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class RegisterService : IRegisterService
    {
        private const string NoUpdatesStr = "No updates were passed";
        private const string ManyUpdatesStr = "There can only be one update per request";
        private const string InvalidIndexStr = "Provided no or invalid account Email";
        private const string SuccessfulUpdateStr = "Account updated successfully";

        private const string NoAccountStr = "There is no registered account with this email";
        private const string AccountFoundStr = "Account fetched successfully";

        private const string InvalidKeyStr = "Provided polster key is invalid";
        private const string GetAccountsStr = "Successfully queried registrations";

        private readonly AnkietyzatorDbContext _context;

        private readonly IKeyService _keyService;
        private readonly IMapper _mapper;

        public RegisterService(AnkietyzatorDbContext context, IMapper mapper, IKeyService keyService)
        {
            _keyService = keyService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<Account>> GetAccount(string email)
        {
            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == email);
            var response = new ServiceResponse<Account>();
            return account == null
                ? response.Failure(NoAccountStr, HttpStatusCode.NotFound)
                : response.Success(account, AccountFoundStr);
        }

        public async Task<ServiceResponse<List<Account>>> GetAccounts()
        {
            var response = new ServiceResponse<List<Account>>();
            var accounts = await _context.Accounts.ToListAsync();
            return response.Success(accounts, GetAccountsStr);
        }

        public async Task<ServiceResponse<Account>> UpdateMyAccount(UpdateAccountDto updateAccountDto,
            HttpContext context)
        {
            var response = await UpdateAccount(updateAccountDto);
            if (response.Data == null) return response;
            await UpdateCurrentIdentity(context, response.Data);
            return response.Success(_mapper.Map<Account>(response.Data), SuccessfulUpdateStr);
        }

        public async Task<ServiceResponse<Account>> UpdateOtherAccount(UpdateAccountDto updateAccountDto)
        {
            var response = await UpdateAccount(updateAccountDto);
            return response.Data == null
                ? response
                : response.Success(_mapper.Map<Account>(response.Data), SuccessfulUpdateStr);
        }

        //======================================= UPDATE =======================================//

        private static bool InvalidPollsterKey(UpdateAccountDto createAccountDto, UpgradeKey key, string eMail)
        {
            string dtoKey = createAccountDto.Key;
            if (dtoKey == null) return false;
            return key == null || key.EMail != eMail && key.EMail.Length > 0;
        }

        private static (bool, bool) GetConditions(UpdateAccountDto dto) => (dto.Tags != null, dto.Key != null);

        private async Task<ServiceResponse<Account>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var response = new ServiceResponse<Account>();
            const HttpStatusCode code = HttpStatusCode.UnprocessableEntity;
            (bool changedTags, bool changedKey) = GetConditions(updateAccountDto);
            int changes = new[] {changedTags, changedKey}.Count(b => b);
            if (changes < 1) return response.Failure(NoUpdatesStr, code);

            Account account = await _context.Accounts.FirstOrDefaultAsync(a => a.EMail == updateAccountDto.EMail);
            if (account == null) return response.Failure(InvalidIndexStr, code);

            var keyResponse = await _keyService.GetUpgradeKey(updateAccountDto.Key);
            if (changedKey && keyResponse.Data == null) return response.Failure(keyResponse);
            var key = keyResponse.Data;

            if (InvalidPollsterKey(updateAccountDto, key, account.EMail))
                return response.Failure(InvalidKeyStr, code);

            await MakeAccountChanges(account, updateAccountDto.Tags, key);
            await _context.SaveChangesAsync();
            return response.Success(account, SuccessfulUpdateStr);
        }

        private async Task MakeAccountChanges(Account account, string tags, UpgradeKey key)
        {
            if (tags != null) account.Tags = tags;
            if (key != null)
            {
                account.UserType = key.UserType;
                _context.UpgradeKeys.Remove(key);
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateCurrentIdentity(HttpContext context, Account account)
        {
            var identities = context.User.Identities;
            var newId = new ClaimsPrincipal();

            newId.AddIdentity(identities.ElementAt(0));
            newId.AddIdentity(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Role, account.UserType.GetRole())}));

            context.User = newId;
            await context.SignOutAsync();
            await context.SignInAsync(newId);
        }
    }
}