using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
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

        [HttpPost("/Financial/{id}/Organisation/{orgId}/UpdateGrade")]
        public async Task<ActionResult> UpdateGrade(Guid id, Guid orgId, [FromBody] FinancialGrade updatedGrade)
        {
            await _mediator.Send(new UpdateGradeRequest(id, orgId,updatedGrade));
            return Ok();

        }

        [HttpPost("/Financial/Application/{Id}/StartReview")]
        public async Task<ActionResult> StartReview(Guid Id)
        {
            await _mediator.Send(new StartFinancialReviewRequest(Id));
            return Ok();
        }
    }
}