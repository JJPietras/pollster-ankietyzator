using System.Collections.Generic;
using System.Net;
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
            return Ok(new Response<List<UpgradeKey>>(keysResponse));
        }
        
        //===================== PUT ========================//
        
        [HttpPut("update-key")]
        //TODO: uncomment [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUpgradeKey(UpdateUpgradeKeyDto upgradeKey)
        {
            var keysResponse = await _keyService.UpdateUpgradeKey(upgradeKey);
            var response = new Response<UpgradeKey>(keysResponse);
            if (keysResponse.Code == HttpStatusCode.UnprocessableEntity) return UnprocessableEntity(response);
            return Ok(response);
        }
        
        //===================== POST =======================//
        
        [HttpPost("add-key")]
        //TODO: uncomment [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUpgradeKey(UpgradeKey upgradeKey)
        {
            var keysResponse = await _keyService.AddUpgradeKey(upgradeKey);
            var response = new Response<UpgradeKey>(keysResponse);
            return keysResponse.Code switch
            {
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.Conflict => Conflict(response),
                _ => Conflict(response)
            };
        }
        
        //===================== DELETE =======================//
        
        [HttpDelete("remove-key/{key}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveUpgradeKey(string key)
        {
            var keysResponse = await _keyService.RemoveUpgradeKey(key);
            var response = new Response<UpgradeKey>(keysResponse);
            if (keysResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
    }
}