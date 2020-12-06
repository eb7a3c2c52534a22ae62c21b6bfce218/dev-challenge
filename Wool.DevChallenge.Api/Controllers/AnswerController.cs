using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wool.DevChallenge.Api.Application.Queries.GetUserQuery;
using Wool.DevChallenge.Api.Exceptions;

namespace Wool.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly ILogger<AnswerController> _logger;
        private readonly IMediator _mediator;

        public AnswerController(ILogger<AnswerController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("")]
        [HttpGet("user")]
        public async Task<ActionResult> GetUser()
        {
            var currentRequest = ControllerContext.HttpContext.Request;
            try
            {
                var data = await _mediator.Send(new GetUserQuery());
                
                if(data == null) 
                    throw new Exception("Unknown error reading user data");

                return Ok(data);
            }
            catch (ApplicationSettingsInvalidException ex)
            {
                _logger.LogError(ex, "Server configuration invalid");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.InternalServerError, "Server configuration invalid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown server internal error");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}
