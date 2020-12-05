using System.Linq;
using System.Threading.Tasks;
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

        //#### USER ####//

        [HttpGet("get-user-un-archived")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetUserUnArchivedPollForms()
        {
            var response = await _polling.GetUserPollForms(GetUserEmail(), false);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-user-archived")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetUserArchivedPollForms()
        {
            var response = await _polling.GetUserPollForms(GetUserEmail(), true);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        //#### ADMIN ####//

        [HttpGet("get-un-archived")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUnArchivedPollForms()
        {
            var response = await _polling.GetAllPollForms(false);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-archived")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetArchivedPollForms()
        {
            var response = await _polling.GetAllPollForms(true);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        //#### POLLSTER ####//

        [HttpGet("get-pollster-un-archived")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetNotArchivedPolls()
        {
            var response = await _polling.GetPollsterPollForms(GetUserEmail(), false);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-pollster-archived")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetArchivedPolls()
        {
            var response = await _polling.GetPollsterPollForms(GetUserEmail(), true);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        //===================== POST =======================//

        [HttpPost("update-poll")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> UpdatePoll(UpdatePollFormDto updatePollFormDto)
        {
            var response = await _polling.UpdatePollForm(updatePollFormDto, GetUserEmail());
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpPost("remove-poll/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> RemovePoll(int pollId)
        {
            var response = await _polling.RemovePollForm(pollId, GetUserEmail());
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpPost("create-poll")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> CreatePoll(CreatePollFormDto createPollFormDto)
        {
            var response = await _polling.CreatePollForm(createPollFormDto, GetUserEmail());
            if (response.Data == null) return Conflict(response);
            return Ok(response);
        }

        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}