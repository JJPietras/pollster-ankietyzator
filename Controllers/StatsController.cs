using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.StatModel;
using Ankietyzator.Models.DTO.PollDTOs;
using Ankietyzator.Models.DTO.StatsDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ankietyzator.Controllers
{
    [Authorize]
    [ApiController]
    [Route("stats")]
    public class StatsController : ControllerBase
    {
        private readonly IStatService _stats;
                
        public StatsController(IStatService stats)
        {
            _stats = stats;
        }
        
        //===================== GET =======================//
        
        [HttpGet("get-poll-stats/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetPollStats(int pollId)
        {
            var statsResponse = await _stats.GetPollStat(pollId);
            var response = new Response<GetPollStatsDto>(statsResponse);
            if (statsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-polls-stats/{pollsterEmail}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPollsStats(string pollsterEmail)
        {
            var statsResponse = await _stats.GetPollsStats(pollsterEmail);
            var response = new Response<List<GetPollStatsDto>>(statsResponse);
            if (statsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-polls-stats")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetPollsStats()
        {
            var statsResponse = await _stats.GetPollsStats(GetUserEmail());
            var response = new Response<List<GetPollStatsDto>>(statsResponse);
            if (statsResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-questions-stats/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetQuestionsStats(int pollId)
        {
            var statsResponse = await _stats.GetQuestionsStats(pollId);
            var response = new Response<List<GetQuestionStatsDto>>(statsResponse);
            return statsResponse.Code switch
            {
                HttpStatusCode.Conflict => Conflict(response),
                HttpStatusCode.NotFound => NotFound(response),
                _ => Ok(response)
            };
        }
        
        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}