using System;
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
        private const string KeyTypeFailStr = "User type must be between 0 and 2 inclusive";
        private const string KeyMailInvalidStr = "EMail can be empty but must not be null";
        private const string KeyExistsStr = "Key with key string or email already exists";
        private const string KeyLengthStr = "Key must have more than 3 characters";
        private const string KeysFetchedStr = "Keys fetched successfully";
        private const string KeyFetchedStr = "Key fetched successfully";
        private const string KeyRemovedStr = "Key removed successfully";
        private const string KeyUpdatedStr = "Key updated successfully";
        private const string KeyAddedStr = "Key added successfully";
        private const string NoKeyStr = "Key not found";

        private readonly AnkietyzatorDbContext _context;

        public KeyService(AnkietyzatorDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<UpgradeKey>>> GetUpgradeKeys()
        {
            var response = new Response<List<UpgradeKey>>();
            return response.Success(await _context.UpgradeKeys.ToListAsync(), KeysFetchedStr);
        }

        public async Task<Response<UpgradeKey>> GetUpgradeKey(string key)
        {
            var response = new Response<UpgradeKey>();
            var dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            return dalKey == null ? response.Failure(NoKeyStr) : response.Success(dalKey, KeyFetchedStr);
        }

        public async Task<Response<UpgradeKey>> RemoveUpgradeKey(string key)
        {
            var response = new Response<UpgradeKey>();
            var dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            if (dalKey == null) return response.Failure(NoKeyStr);
            _context.UpgradeKeys.Remove(dalKey);
            await _context.SaveChangesAsync();
            return response.Success(dalKey, KeyRemovedStr);
        }

        public async Task<Response<UpgradeKey>> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var response = new Response<UpgradeKey>();
            if (!Enum.IsDefined(typeof(UserType), upgradeKey.UserType)) return response.Failure(KeyTypeFailStr);
            if (upgradeKey.Key == null || upgradeKey.Key.Length < 4) return response.Failure(KeyLengthStr);
            if (upgradeKey.EMail == null) return response.Failure(KeyMailInvalidStr);

            return await UpdateSecureKey(upgradeKey);
        }

        public async Task<Response<UpgradeKey>> AddUpgradeKey(UpgradeKey upgradeKey)
        {
            var response = new Response<UpgradeKey>();
            if (upgradeKey.Key.Length < 4) return response.Failure(KeyLengthStr);
            var existingKey = await _context.UpgradeKeys.FirstOrDefaultAsync(
                u => u.Key == upgradeKey.Key || u.EMail == upgradeKey.EMail && u.EMail != ""
            );
            if (existingKey != null) return response.Failure(KeyExistsStr);
            await _context.UpgradeKeys.AddAsync(upgradeKey);
            await _context.SaveChangesAsync();
            return response.Success(upgradeKey, KeyAddedStr);
        }

        //===================================== HELPER METHODS =====================================// 

        private async Task<Response<UpgradeKey>> UpdateSecureKey(UpdateUpgradeKeyDto keyDto)
        {
            var response = new Response<UpgradeKey>();
            UpgradeKey dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.EMail == keyDto.EMail);

            if (dalKey != null) dalKey.Key = keyDto.Key;
            else
            {
                dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == keyDto.Key);
                if (dalKey == null) return response.Failure(NoKeyStr);
                dalKey.EMail = keyDto.EMail;
            }

            dalKey.UserType = keyDto.UserType;

            await _context.SaveChangesAsync();
            return response.Success(dalKey, KeyUpdatedStr);
        }
    }
}