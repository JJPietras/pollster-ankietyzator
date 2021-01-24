using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<ServiceResponse<List<UpgradeKey>>> GetUpgradeKeys()
        {
            var response = new ServiceResponse<List<UpgradeKey>>();
            return response.Success(await _context.UpgradeKeys.ToListAsync(), KeysFetchedStr);
        }

        public async Task<ServiceResponse<UpgradeKey>> GetUpgradeKey(string key)
        {
            var response = new ServiceResponse<UpgradeKey>();
            var dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            return dalKey == null
                ? response.Failure(NoKeyStr, HttpStatusCode.NotFound)
                : response.Success(dalKey, KeyFetchedStr);
        }

        public async Task<ServiceResponse<UpgradeKey>> RemoveUpgradeKey(string key)
        {
            var response = new ServiceResponse<UpgradeKey>();
            var dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == key);
            if (dalKey == null) return response.Failure(NoKeyStr, HttpStatusCode.NotFound);
            _context.UpgradeKeys.Remove(dalKey);
            await _context.SaveChangesAsync();
            return response.Success(dalKey, KeyRemovedStr);
        }

        public async Task<ServiceResponse<UpgradeKey>> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var response = new ServiceResponse<UpgradeKey>();
            const HttpStatusCode code = HttpStatusCode.UnprocessableEntity;
            if (!Enum.IsDefined(typeof(UserType), upgradeKey.UserType)) return response.Failure(KeyTypeFailStr, code);
            if (upgradeKey.Key == null || upgradeKey.Key.Length < 4) return response.Failure(KeyLengthStr, code);
            if (upgradeKey.EMail == null) return response.Failure(KeyMailInvalidStr, code);

            return await UpdateSecureKey(upgradeKey);
        }

        public async Task<ServiceResponse<UpgradeKey>> AddUpgradeKey(UpgradeKey upgradeKey)
        {
            var response = new ServiceResponse<UpgradeKey>();
            if (upgradeKey.Key.Length < 4) return response.Failure(KeyLengthStr, HttpStatusCode.UnprocessableEntity);
            var existingKey = await _context.UpgradeKeys.FirstOrDefaultAsync(
                u => u.Key == upgradeKey.Key || u.EMail == upgradeKey.EMail && u.EMail != ""
            );
            if (existingKey != null) return response.Failure(KeyExistsStr, HttpStatusCode.Conflict);
            await _context.UpgradeKeys.AddAsync(upgradeKey);
            await _context.SaveChangesAsync();
            return response.Success(upgradeKey, KeyAddedStr);
        }

        //===================================== HELPER METHODS =====================================// 

        private async Task<ServiceResponse<UpgradeKey>> UpdateSecureKey(UpdateUpgradeKeyDto keyDto)
        {
            var response = new ServiceResponse<UpgradeKey>();
            UpgradeKey dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.EMail == keyDto.EMail);

            if (dalKey != null) dalKey.Key = keyDto.Key;
            else
            {
                dalKey = await _context.UpgradeKeys.FirstOrDefaultAsync(k => k.Key == keyDto.Key);
                if (dalKey == null) return response.Failure(NoKeyStr, HttpStatusCode.NotFound);
                dalKey.EMail = keyDto.EMail;
            }

            dalKey.UserType = keyDto.UserType;

            await _context.SaveChangesAsync();
            return response.Success(dalKey, KeyUpdatedStr);
        }
    }
}