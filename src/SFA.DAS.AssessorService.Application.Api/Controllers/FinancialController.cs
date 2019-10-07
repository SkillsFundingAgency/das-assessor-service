using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial.Review;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [ValidateBadRequest]
    public class FinancialController : Controller
    {
        private readonly IMediator _mediator;
        
        public FinancialController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/Financial/OpenApplications")]
        public async Task<ActionResult> OpenApplications()
        {
            var applications =await _mediator.Send(new OpenFinancialApplicationsRequest());
            return Ok(applications);
        }

        [HttpGet("/Financial/FeedbackAddedApplications")]
        public async Task<ActionResult> FeedbackAddedApplications()
        {
            var applications = await _mediator.Send(new FeedbackAddedFinancialApplicationsRequest());
            return Ok(applications);

        }

        [HttpGet("/Financial/ClosedApplications")]
        public async Task<ActionResult> ClosedApplications()
        {
            var applications = await _mediator.Send(new ClosedFinancialApplicationsRequest());
            return Ok(applications);

        }

        [HttpPost("/Financial/{Id}/Return")]
        public async Task<ActionResult> ReturnReview(Guid Id, [FromBody] FinancialGrade updatedGrade)
        {
            await _mediator.Send(new ReturnFinancialReviewRequest(Id, updatedGrade));
            return Ok();

        }

        [HttpPost("/Financial/{Id}/StartReview")]
        public async Task<ActionResult> StartReview(Guid Id, [FromBody] StartFinancialReviewRequest request)
        {
            await _mediator.Send(new AssessorService.Api.Types.Models.Apply.Financial.Review.StartFinancialReviewRequest(Id, request.StartedBy));
            return Ok();
        }

        public class StartFinancialReviewRequest
        {
            public string StartedBy { get; set; }
        }
    }
}