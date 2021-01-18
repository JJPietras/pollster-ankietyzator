using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        //#### USER ####//

        [HttpGet("get-user-un-filled")]
        [Authorize(Roles = "user, admin")]
        public async Task<IActionResult> GetUserUnFilledPollForms()
        {
            var pollsResponse = await _polling.GetUserPollForms(GetUserEmail(), false);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-user-filled")]
        [Authorize(Roles = "user, admin")] //TODO: remove admin from both
        public async Task<IActionResult> GetUserFilledPollForms()
        {
            var pollsResponse = await _polling.GetUserPollForms(GetUserEmail(), true);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        //#### ADMIN ####//

        [HttpGet("get-un-archived")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUnArchivedPollForms()
        {
            var pollsResponse = await _polling.GetAllPollForms(false);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-archived")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetArchivedPollForms()
        {
            var pollsResponse = await _polling.GetAllPollForms(true);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        //#### POLLSTER ####//

        [HttpGet("get-pollster-un-archived")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetNotArchivedPolls()
        {
            var pollsResponse = await _polling.GetPollsterPollForms(GetUserEmail(), false);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-pollster-archived")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetArchivedPolls()
        {
            var pollsResponse = await _polling.GetPollsterPollForms(GetUserEmail(), true);
            var response = new Response<List<GetPollFormDto>>(pollsResponse);
            if (pollsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("clone-poll/{pollsterId}/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> ClonePoll(int pollsterId, int pollId)
        {
            var pollsResponse = await _polling.ClonePollForm(GetUserEmail(), pollsterId, pollId);
            var response = new Response<GetPollFormDto>(pollsResponse);
            return pollsResponse.Code switch
            {
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.Unauthorized => Unauthorized(response),
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                _ => Ok(response)
            };
        }

        //===================== PUT =======================//

        [HttpPut("update-poll")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> UpdatePoll(UpdatePollFormDto updatePollFormDto)
        {
            var pollsResponse = await _polling.UpdatePollForm(updatePollFormDto, GetUserEmail());
            var response = new Response<GetPollFormDto>(pollsResponse);
            return pollsResponse.Code switch
            {
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.Unauthorized => Unauthorized(response),
                _ => Ok(response)
            };
        }
        
        [HttpPut("close-poll/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> ClosePollForm(int pollId)
        {
            var pollsResponse = await _polling.ClosePollForm(pollId, GetUserEmail());
            var response = new Response<GetPollFormDto>(pollsResponse);
            return pollsResponse.Code switch
            {
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.Unauthorized => Unauthorized(response),
                _ => Ok(response)
            };
        }
        
        //===================== POST =======================//

        [HttpPost("create-poll")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> CreatePoll(CreatePollFormDto createPollFormDto)
        {
            var pollsResponse = await _polling.CreatePollForm(createPollFormDto, GetUserEmail());
            var response = new Response<GetPollFormDto>(pollsResponse);
            return pollsResponse.Code switch
            {
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.NotFound => NotFound(response),
                _ => Ok(response)
            };
        }
        
        //===================== DELETE =======================//
        
        [HttpDelete("remove-poll/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> RemovePoll(int pollId)
        {
            var pollsResponse = await _polling.RemovePollForm(pollId, GetUserEmail());
            var response = new Response<GetPollFormDto>(pollsResponse);
            return pollsResponse.Code switch
            {
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.Unauthorized => Unauthorized(response),
                _ => Ok(response)
            };
        }
        
        //===================== UTILITY =======================//

        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}