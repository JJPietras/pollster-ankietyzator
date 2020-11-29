using System.Collections.Generic;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.KeyDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Services.Implementations
{
    public class KeyService : IKeyService
    {
        private const string KeysFetchedStr = "Keys fetched successfuly";
        private const string KeyFetchedStr = "Key fetched successfuly";
        private const string KeyRemovedStr = "Key removed successfuly";
        private const string KeyUpdatedStr = "Key updated successfuly";
        private const string KeyAddedStr = "Key added successfuly";
        private const string NoKeyStr = "Key not found";

        public AnkietyzatorDbContext Context { get; set; }

        public async Task<Response<List<UpgradeKey>>> GetPollsterKeys()
        {
            var response = new Response<List<UpgradeKey>>();
            return response.Success(await Context.UpgradeKeys.ToListAsync(), KeysFetchedStr);
        }

        public async Task<Response<UpgradeKey>> GetPollsterKey(string key)
        {
            var response = new Response<UpgradeKey>();
            var dalKey = await Context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            return dalKey == null ? response.Failure(NoKeyStr) : response.Success(dalKey, KeyFetchedStr);
        }

        public async Task<Response<UpgradeKey>> RemovePollsterKey(string key)
        {
            var response = new Response<UpgradeKey>();
            var dalKey = await Context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            if (dalKey == null) return response.Failure(NoKeyStr);
            Context.UpgradeKeys.Remove(dalKey);
            await Context.SaveChangesAsync();
            return response.Success(dalKey, KeyRemovedStr);
        }

        public async Task<Response<UpgradeKey>> UpdatePollsterKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var response = new Response<UpgradeKey>();
            UpgradeKey dalKey = await Context.UpgradeKeys.FirstOrDefaultAsync(k => k.EMail == upgradeKey.EMail);
            if (dalKey != null) dalKey.Key = upgradeKey.Key;
            else
            {
                dalKey = await Context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == upgradeKey.Key);
                if (dalKey == null) return response.Failure(NoKeyStr);
                dalKey.EMail = upgradeKey.EMail;
            }

            if (dalKey.UserType != upgradeKey.UserType) dalKey.UserType = upgradeKey.UserType;

            await Context.SaveChangesAsync();
            return response.Success(dalKey, KeyUpdatedStr);
        }

        public async Task<Response<object>> AddPollsterKey(UpgradeKey upgradeKey)
        {
            await Context.UpgradeKeys.AddAsync(upgradeKey);
            await Context.SaveChangesAsync();
            return new Response<object>().Success(null, KeyAddedStr);
        }
    }
}