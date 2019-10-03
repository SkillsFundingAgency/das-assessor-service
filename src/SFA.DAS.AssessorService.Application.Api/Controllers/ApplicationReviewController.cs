using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    public class ApplicationReviewController : Controller
    {
        private readonly ILogger<ApplicationReviewController> _logger;
        private readonly IMediator _mediator;

        public ApplicationReviewController(ILogger<ApplicationReviewController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("Review/OpenApplications")]
        public async Task<ActionResult> OpenApplications(int sequenceNo = 1)
        {
            var applications = await _mediator.Send(new OpenApplicationsRequest(sequenceNo));
            return Ok(applications);
        }

        [HttpGet("Review/FeedbackAddedApplications")]
        public async Task<ActionResult> FeedbackAddedApplications()
        {
            var applications = await _mediator.Send(new FeedbackAddedApplicationsRequest());
            return Ok(applications);
        }

        [HttpGet("Review/ClosedApplications")]
        public async Task<ActionResult> ClosedApplications()
        {
            var applications = await _mediator.Send(new ClosedApplicationsRequest());
            return Ok(applications);
        }

        [HttpPost("/Applications/{applicationId}/Sequences/{sequenceId}/Return")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task ReturnApplication(Guid applicationId, int sequenceId, [FromBody] ReturnApplicationRequest request)
        {
            _logger.LogInformation($"Received request to return application");
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Review.ReturnApplicationRequest(applicationId, sequenceId, request.ReturnType));
        }

        public class ReturnApplicationRequest
        {
            public string ReturnType { get; set; }
        }
    }
}