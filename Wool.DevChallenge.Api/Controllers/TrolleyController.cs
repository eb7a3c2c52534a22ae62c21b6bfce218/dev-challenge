using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Wool.DevChallenge.Api.Exceptions;
using Wool.DevChallenge.Api.Models.Trolley;


namespace Wool.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TrolleyTotalController : ControllerBase
    {
        private readonly ILogger<TrolleyTotalController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TrolleyTotalController(IMediator mediator, IMapper mapper, ILogger<TrolleyTotalController> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> TrolleyTotal([FromBody] TrolleyInputModel model)
        {
            var currentRequest = ControllerContext.HttpContext.Request;
            try
            {
                var trolley = _mapper.Map<CalculateTrolleyTotalCommand.RequestTrolley>(model);

                var data = await _mediator.Send(new CalculateTrolleyTotalCommand(trolley, TrolleyCalculatorType.Remote));

                return Ok(data);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Trolley is in invalid state");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Trolley State");
            }
            catch (InvalidTrolleyStateException ex)
            {
                _logger.LogError(ex, "Trolley is in invalid state");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Trolley State");
            }
            catch (TrolleyCalculationRemoteException ex)
            {
                _logger.LogError(ex, "Remote error while calculating trolley total");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)ex.StatusCode, "Remote Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate trolley error");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost("localTrolleyTotal")]
        public async Task<ActionResult> LocalTrolleyTotal([FromBody] TrolleyInputModel model)
        {
            var currentRequest = ControllerContext.HttpContext.Request;
            try
            {
                var trolley = _mapper.Map<CalculateTrolleyTotalCommand.RequestTrolley>(model);

                var data = await _mediator.Send(new CalculateTrolleyTotalCommand(trolley, TrolleyCalculatorType.Local));

                return Ok(data);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Trolley is in invalid state");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Trolley State");
            }
            catch (InvalidTrolleyStateException ex)
            {
                _logger.LogError(ex, "Trolley is in invalid state");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.BadRequest, "Invalid Trolley State");
            }
            catch (TrolleyCalculationRemoteException ex)
            {
                _logger.LogError(ex, "Remote error while calculating trolley total");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)ex.StatusCode, "Remote Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate trolley error");
                return Problem(ex.Message, currentRequest.Path.ToString(), (int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }


    }
}
