using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/oppfinder")]
    [ValidateBadRequest]
    public class OppFinderController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OppFinderController> _logger;

        public OppFinderController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<OppFinderController> logger)
            : base(taskQueue, logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("expression-of-interest", Name = "CreateExpressionOfInterest")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateExpressionOfInterest([FromBody] OppFinderExpressionOfInterestRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Expression of interest");
                return Ok(await _mediator.Send(request));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("update-standard-summary", Name = "UpdateStandardSummary")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult UpdateStandardSummary()
        {
            return QueueBackgroundRequest(new UpdateStandardSummaryRequest(), "update standard summary");
        }
    }
}
