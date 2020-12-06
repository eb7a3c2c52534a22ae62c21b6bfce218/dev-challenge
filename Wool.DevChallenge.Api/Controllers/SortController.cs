using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery;
using Wool.DevChallenge.Api.Exceptions;
using Wool.DevChallenge.Api.Models;

namespace Wool.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class SortController : ControllerBase
    {
        private readonly ILogger<SortController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SortController(IMediator mediator, IMapper mapper, ILogger<SortController> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Sort([FromQuery] SortOption sortOption)
        {

            var currentRequest = ControllerContext.HttpContext.Request;
            try
            {
                var productSortOption = _mapper.Map<ProductSortOption>(sortOption);

                var data = await _mediator.Send(new GetSortedProductsQuery(productSortOption));

                if (data == null || !data.Any())
                    return NoContent();

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
