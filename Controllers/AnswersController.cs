using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ankietyzator.Models;
using Ankietyzator.Models.DataModel.AccountModel;
using Ankietyzator.Models.DataModel.PollModel;
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
            var answersResponse = await _answer.GetAnswers(GetUserEmail(), pollId, null);
            var response = new Response<List<GetAnswerDto>>(answersResponse);
            if (answersResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-answers/{pollId}/{userMail}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetUserAnswers(int pollId, string userMail)
        {
            var answersResponse = await _answer.GetAnswers(userMail, pollId, GetUserEmail());
            var response = new Response<List<GetAnswerDto>>(answersResponse);
            if (answersResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }
        
        [HttpGet("get-detailed-answers/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetDetailedAnswers(int pollId)
        {
            var answersResponse = await _answer.GetDetailedAnswers(pollId);
            var response = new Response<List<GetDetailedAnswerDto>>(answersResponse);
            if (answersResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("get-anonymous-answers/{pollId}")]
        [Authorize(Roles = "pollster, admin")]
        public async Task<IActionResult> GetAnonymousAnswers(int pollId)
        {
            var answersResponse = await _answer.GetAnonymousAnswers(pollId);
            var response = new Response<List<GetAnswerDto>>(answersResponse);
            if (answersResponse.Code == HttpStatusCode.NotFound) return NotFound(response);
            return Ok(response);
        }

        //======================= POST ========================//
        
        [HttpPost("add-answers")]
        [Authorize(Roles = "user, pollster, admin")]
        public async Task<IActionResult> AddAnswers(List<CreateAnswerDto> answerDtos)
        {
            var answersResponse = await _answer.AddAnswers(answerDtos, GetUserEmail());
            var response = new Response<List<GetAnswerDto>>(answersResponse);
            return answersResponse.Code switch
            {
                HttpStatusCode.NotFound => NotFound(response),
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(response),
                HttpStatusCode.Conflict => Conflict(response),
                _ => Ok(response)
            };
        }
        
        //===================== UTILITY =======================//
        
        private string GetUserEmail() => HttpContext.User.Claims.ElementAt(2).Value;
    }
}