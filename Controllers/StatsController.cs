using System.Linq;
using System.Threading.Tasks;
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
            var response = await _stats.GetPollStat(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-polls-stats/{pollsterEmail}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPollsStats(string pollsterEmail)
        {
            var response = await _stats.GetPollsStats(pollsterEmail);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-polls-stats")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetPollsStats()
        {
            var response = await _stats.GetPollsStats(GetUserEmail());
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-questions-stats/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetQuestionsStats(int pollId)
        {
            var response = await _stats.GetQuestionsStats(pollId);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}