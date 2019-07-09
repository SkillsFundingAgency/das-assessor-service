using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/externalapi")]
    [ValidateBadRequest]
    public class ExternalApiController : Controller
    {
        private readonly IMediator _mediator;

        public ExternalApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("learnerdetails")]
        public async Task<IActionResult> GetLearnerDetails(string standard, long uln)
        {
            var request = new GetBatchLearnerRequest
            {
                Uln = uln,
                Standard = standard,
                IncludeCertificate = false,
            };

            var learnerDetails = await _mediator.Send(request);

            return Ok(learnerDetails.Learner);
        }
    }
}
