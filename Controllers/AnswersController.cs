using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankietyzator.Models.DTO.AnswerDTOs;
using Ankietyzator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ankietyzator.Controllers
{
    [Authorize]
    [ApiController]
    [Route("answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answer;

        public AnswersController(IAnswerService answer)
        {
            _answer = answer;
        }
        
        //===================== GET =======================//

        [HttpGet("get-answers/{pollId}")]
        [Authorize(Roles = "user, admin")] //TODO: remove admin
        public async Task<IActionResult> GetMyAnswers(int pollId)
        {
            var response = await _answer.GetAnswers(GetUserEmail(), pollId, null);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-answers/{pollId}/{userMail}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetUserAnswers(int pollId, string userMail)
        {
            Console.WriteLine("here");
            var response = await _answer.GetAnswers(userMail, pollId, GetUserEmail());
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        //======================= POST ========================//
        
        [HttpPost("add-answers")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> AddAnswers(List<CreateAnswerDto> answerDtos)
        {
            Console.WriteLine("here");
            var response = await _answer.AddAnswers(answerDtos, GetUserEmail());
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
        
        //===================== UTILITY =======================//
        
        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}