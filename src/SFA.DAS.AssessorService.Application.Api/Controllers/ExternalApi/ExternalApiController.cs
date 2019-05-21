using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi;
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
            return Ok(await _mediator.Send(new LearnerDetailForExternalApiRequest(standard, uln)));
        }
    }
}
