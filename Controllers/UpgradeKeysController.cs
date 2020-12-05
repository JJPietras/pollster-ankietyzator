using System.Threading.Tasks;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DTO.KeyDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ankietyzator.Controllers
{
    [Authorize]
    [ApiController]
    [Route("keys")]
    public class UpgradeKeysController : ControllerBase
    {
        private readonly IKeyService _keyService;

        public UpgradeKeysController(IKeyService keyService)
        {
            _keyService = keyService;
        }
        
        //===================== GET =======================//
        
        /*[HttpGet("get-key")]
        public async Task<IActionResult> GetUpgradeKey(string key)
        {
            var keysResponse = await _keyService.GetPollsterKey(key);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }*/
        
        [HttpGet("get-keys")]
        //TODO: uncomment [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUpgradeKeys()
        {
            var keysResponse = await _keyService.GetUpgradeKeys();
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        //===================== PUT ========================//
        
        [HttpPut("update-key")]
        //TODO: uncomment [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var keysResponse = await _keyService.UpdateUpgradeKey(upgradeKey);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        //===================== POST =======================//
        
        [HttpPost("add-key")]
        //TODO: uncomment [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUpgradeKey(UpgradeKey upgradeKey)
        {
            var keysResponse = await _keyService.AddUpgradeKey(upgradeKey);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return Conflict(keysResponse);
        }
        
        //===================== DELETE =======================//
        
        [HttpDelete("remove-key/{key}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveUpgradeKey(string key)
        {
            var keysResponse = await _keyService.RemoveUpgradeKey(key);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
    }
}