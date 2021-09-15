using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/approvals/")]
    [ValidateBadRequest]
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IMediator _mediator;

        public ApprovalsController(ILogger<ApprovalsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("update-approvals", Name = "update-approvals/GatherAndStoreApprovals")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ApiResponse))]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        //[SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GatherAndStoreApprovals([FromBody] ImportApprovalsRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
