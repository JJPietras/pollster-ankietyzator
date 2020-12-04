using System.Threading.Tasks;
using Ankietyzator.Models;
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
        
        [HttpGet("get-poll-stats")]
        public async Task<IActionResult> GetPollStats(int pollId)
        {
            //TODO: authorize
            var response = await _stats.GetPollStat(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-polls-stats")]
        public async Task<IActionResult> GetPollsStats(int pollsterId)
        {
            //TODO: authorize
            var response = await _stats.GetPollsStats(pollsterId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-questions-stats")]
        public async Task<IActionResult> GetQuestionsStats(int pollId)
        {
            //TODO: authorize
            var response = await _stats.GetQuestionsStats(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
    }
}