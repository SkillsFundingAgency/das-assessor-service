using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.Staff;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/learnerdetails")]
    [ValidateBadRequest]
    public class LearnerDetailsController : Controller
    {
        private readonly IMediator _mediator;

        public LearnerDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int stdCode, 
            long uln,
            string certificateReference,
            bool allLogs=false)
        {
            return Ok(await _mediator.Send(new LearnerDetailRequest(certificateReference,
                stdCode,
                uln,
                allLogs)));
        }
    }
}