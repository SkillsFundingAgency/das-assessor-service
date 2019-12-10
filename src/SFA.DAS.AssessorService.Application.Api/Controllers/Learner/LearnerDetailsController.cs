using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.Learner;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/learnerdetails")]
    [ValidateBadRequest]
    public class LearnerDetailsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LearnerDetailsController> _logger;

        public LearnerDetailsController(IMediator mediator, ILogger<LearnerDetailsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("import", Name = "ImportLearnerDetail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> ImportLearnerDetail([FromBody] ImportLearnerDetailRequest request)
        {
            try
            {
                _logger.LogInformation("Importing a learner detail record");
                return Ok(await _mediator.Send(request));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Internal server error, Message: [{ex.Message}]");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
