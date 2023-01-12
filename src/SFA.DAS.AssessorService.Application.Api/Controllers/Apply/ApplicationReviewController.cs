using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Apply
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

        [HttpGet("Review/ApplicationReviewStatusCounts")]
        public async Task<ActionResult> ApplicationReviewStatusCounts()
        {
            var applicationReviewStatusCounts = await _mediator.Send(new ApplicationReviewStatusCountsRequest());
            return Ok(applicationReviewStatusCounts);
        }

        [HttpPost("Review/OrganisationApplications")]
        public async Task<ActionResult> OrganisationApplications([FromBody] OrganisationApplicationsRequest organisationApplicationsRequest)
        {
            var applications = await _mediator.Send(organisationApplicationsRequest);
            return Ok(applications);
        }

        [HttpPost("Review/WithdrawalApplications")]
        public async Task<ActionResult> WithdrawalApplications([FromBody] WithdrawalApplicationsRequest withdrawalApplicationsRequest)
        {
            var applications = await _mediator.Send(withdrawalApplicationsRequest);
            return Ok(applications);
        }

        [HttpPost("Review/StandardApplications")]
        public async Task<ActionResult> StandardApplications([FromBody] StandardApplicationsRequest standardApplicationsRequest)
        {
            var applications = await _mediator.Send(standardApplicationsRequest);
            return Ok(applications);
        }

        [HttpPost("Review/Applications/{Id}/Sequences/{sequenceId}/Return")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task ReturnApplicationSequence(Guid Id, int sequenceId, [FromBody] ReturnApplicationSequenceRequest request)
        {
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Review.ReturnApplicationSequenceRequest(Id, sequenceId, request.ReturnType, request.ReturnedBy));
        }

        [HttpPost("Review/Applications/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}/StartReview")]
        public async Task StartSectionReview(Guid Id, int sequenceNo, int sectionNo, [FromBody] StartApplicationSectionReviewRequest request)
        {
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Review.StartApplicationSectionReviewRequest(Id, sequenceNo, sectionNo, request.Reviewer));
        }

        [HttpPost("Review/Applications/{Id}/Sequences/{sequenceNo}/Sections/{sectionNo}/Evaluate")]
        public async Task EvaluateSection(Guid Id, int sequenceNo, int sectionNo, [FromBody] EvaluateApplicationSectionRequest request)
        {
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Review.EvaluateApplicationSectionRequest(Id, sequenceNo, sectionNo, request.IsSectionComplete, request.EvaluatedBy));
        }

        public class StartApplicationSectionReviewRequest
        {
            public string Reviewer { get; set; }
        }

        public class ReturnApplicationSequenceRequest
        {
            public string ReturnType { get; set; }
            public string ReturnedBy { get; set; }
        }

        public class EvaluateApplicationSectionRequest
        {
            public bool IsSectionComplete { get; set; }
            public string EvaluatedBy { get; set; }
        }
    }
}