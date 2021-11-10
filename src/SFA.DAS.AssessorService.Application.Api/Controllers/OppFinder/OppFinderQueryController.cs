using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/oppfinder")]
    [ValidateBadRequest]
    public class OppFinderQueryController : Controller
    {
        private readonly ILogger<OppFinderQueryController> _logger;
        private readonly IMediator _mediator;

        public OppFinderQueryController(IMediator mediator, ILogger<OppFinderQueryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("filter", Name = "GetFilterStandards")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(GetOppFinderFilterStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetFilterStandards([FromBody] GetOppFinderFilterStandardsRequest request)
        {
            _logger.LogInformation($"Received request to retrieve filter standards");
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("approved", Name = "GetApprovedStandards")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(GetOppFinderApprovedStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetApprovedStandards([FromBody] GetOppFinderApprovedStandardsRequest request)
        {
            _logger.LogInformation($"Received request to retrieve approved standards");
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("nonapproved", Name = "GetNonApprovedStandards")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(GetOppFinderNonApprovedStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetNonApprovedStandards([FromBody] GetOppFinderNonApprovedStandardsRequest request)
        {
            _logger.LogInformation($"Received request to retrieve non approved standards {request.NonApprovedType}");
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("approved-details", Name = "GetApprovedStandardDetails")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(GetOppFinderApprovedStandardDetailsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetApprovedStandardDetails([FromBody] GetOppFinderApprovedStandardDetailsRequest request)
        {
            _logger.LogInformation($"Received request to retrieve approved standard details {request.StandardReference}");
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("nonapproved-details", Name = "GetNonApprovedStandardDetails")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(GetOppFinderNonApprovedStandardDetailsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetNonApprovedStandardDetails([FromBody] GetOppFinderNonApprovedStandardDetailsRequest request)
        {
            _logger.LogInformation($"Received request to retrieve non approved standard details {request.StandardReference}");
            return Ok(await _mediator.Send(request));
        }
    }
}