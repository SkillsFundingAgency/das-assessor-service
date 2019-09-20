using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/applications")]
    [ValidateBadRequest]
    public class ApplicationController : Controller
    {
        private readonly ILogger<ApplicationController> _logger;
        private readonly IMediator _mediator;

        public ApplicationController(ILogger<ApplicationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("{userId}/Organisation", Name = "GetOrganisationApplications")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ApplicationResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<List<ApplicationResponse>>> GetOrganisationApplications(string userId)
        {
            _logger.LogInformation($"Received request to retrieve application for organisation");
            return Ok(await _mediator.Send(new GetApplicationsRequest(Guid.Parse(userId), false)));
        }

        [HttpGet("{userId}", Name = "GetApplications")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ApplicationResponse>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<List<ApplicationResponse>>> GetApplications(string userId)
        {
            _logger.LogInformation($"Received request to retrieve application for user");
            return Ok(await _mediator.Send(new GetApplicationsRequest(Guid.Parse(userId), true)));
        }

        [HttpGet("{Id}/application", Name = "GetApplication")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ApplicationResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<ApplicationResponse>> GetApplication(string Id)
        {
            _logger.LogInformation($"Received request to retrieve application for user");
            return Ok(await _mediator.Send(new GetApplicationRequest(Guid.Parse(Id))));
        }

        [HttpPost("createApplication", Name = "CreateApplication")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(Guid))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<Guid>> CreateApplication(
          [FromBody] CreateApplicationRequest createApplicationRequest)
        {
            _logger.LogInformation("Received Create Application Request");

            var applicationResponse = await _mediator.Send(createApplicationRequest);

            return CreatedAtRoute("CreateApplication",
                applicationResponse);
        }

        [HttpPost("submitApplication", Name = "SubmitApplication")]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult<Guid>> SubmitApplication(
         [FromBody] SubmitApplicationRequest submitApplicationRequest)
        {
            _logger.LogInformation("Received Submit Application Request");

            var response = await _mediator.Send(submitApplicationRequest);

            return CreatedAtRoute("SubmitApplication",
                response);
        }

        [HttpPost("updateInitialStandardData", Name = "UpdateInitialStandardData")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<ActionResult> UpdateInitialStandardData(
        [FromBody] UpdateInitialStandardDataRequest updateInitialStandardDataRequest)
        {
            _logger.LogInformation("Received Update Application Standard  Request");

            return Ok(await _mediator.Send(updateInitialStandardDataRequest));
        }
    }
}