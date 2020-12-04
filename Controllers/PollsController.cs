using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DTO.PollDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ankietyzator.Controllers
{
    [Authorize]
    [ApiController]
    [Route("polls")]
    public class PollsController : ControllerBase
    {
        private readonly IPollingService _polling;
                
        public PollsController(IPollingService polling)
        {
            _polling = polling;
        }
        
        //===================== GET =======================//
        
        [HttpGet("get-polls")]
        public async Task<IActionResult> GetPollForms(int pollsterId)
        {
            //TODO: authorize
            var response = await _polling.GetPollForms(pollsterId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-poll/{pollId}")]
        public async Task<IActionResult> GetPollForm(int pollId)
        {
            //TODO: authorize
            var response = await _polling.GetPollForm(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-archived-polls")]
        public async Task<IActionResult> GetArchivedPolls(int pollsterId)
        {
            //TODO: authorize
            var response = await _polling.GetArchivedPollForms(pollsterId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-not-archived-polls")]
        public async Task<IActionResult> GetNotArchivedPolls(int pollsterId)
        {
            //TODO: authorize
            var response = await _polling.GetNotArchivedPollForms(pollsterId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        //===================== POST =======================//
        
        [HttpPost("update-poll")]
        public async Task<IActionResult> UpdatePoll(UpdatePollFormDto updatePollFormDto, int accountId)
        {
            //TODO: authorize
            var response = await _polling.UpdatePollForm(updatePollFormDto, accountId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpPost("remove-poll")]
        public async Task<IActionResult> RemovePoll(int pollId)
        {
            //TODO: authorize
            var response = await _polling.RemovePollForm(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpPost("create-poll")]
        public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest body)
        {
            //TODO: authorize
            var response = await _polling.CreatePollForm(body.PollForm, body.AccountId);
            if (response.Data == null) return Conflict(response);
            return Ok(response);
        }
    }
}