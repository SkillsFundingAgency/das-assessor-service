using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Handlers.Approvals;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/approvals/")]
    [ValidateBadRequest]
    public class ApprovalsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApprovalsController> _logger;

        public ApprovalsController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<ApprovalsController> logger)
            : base(taskQueue, logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("update-approvals", Name = "update-approvals/GatherAndStoreApprovals")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult GatherAndStoreApprovals()
        {
            var requestName = "gather and store approvals";
            return QueueBackgroundRequest(new ImportApprovalsRequest(), requestName, (response, duration, log) =>
            {
                log.LogInformation($"Completed request to {requestName} in {duration.ToReadableString()}");
            });
        }

        [HttpGet("learner", Name = "learner/GetLearner")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetLearner(int stdCode, long uln)
        {
            var record = await _mediator.Send(new GetApprovalsLearnerRecordRequest(stdCode, uln));

            if (record == null) return NotFound();

            return Ok(record);
        }
    }
}
