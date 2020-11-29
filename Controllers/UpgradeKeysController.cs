using System.Threading.Tasks;
using Ankietyzator.Models;
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

        public UpgradeKeysController(AnkietyzatorDbContext context, IKeyService keyService)
        {
            _keyService = keyService;
            _keyService.Context = context;
        }
        
        //===================== GET =======================//
        
        [HttpGet("get-key")]
        public async Task<IActionResult> GetUpgradeKey(string key)
        {
            var keysResponse = await _keyService.GetPollsterKey(key);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        [HttpGet("get-keys")]
        public async Task<IActionResult> GetUpgradeKeys()
        {
            var keysResponse = await _keyService.GetPollsterKeys();
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        //===================== PUT ========================//
        
        [HttpPut("update-key")]
        public async Task<IActionResult> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var keysResponse = await _keyService.UpdatePollsterKey(upgradeKey);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        //===================== POST =======================//
        
        [HttpPost("remove-key")]
        public async Task<IActionResult> RemoveUpgradeKey(string key)
        {
            var keysResponse = await _keyService.RemovePollsterKey(key);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return NotFound(keysResponse);
        }
        
        [HttpPost("add-key")]
        public async Task<IActionResult> AddUpgradeKey(UpgradeKey upgradeKey)
        {
            var keysResponse = await _keyService.AddPollsterKey(upgradeKey);
            if (keysResponse.Data != null) return Ok(keysResponse);
            return Conflict(keysResponse);
        }
    }
}