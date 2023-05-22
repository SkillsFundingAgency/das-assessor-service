using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
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
    public class ApprovalsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ApprovalsController> _logger;

        public ApprovalsController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<ApprovalsController> logger)
        {
            _mediator = mediator;
            _taskQueue = taskQueue;
            _logger = logger;
            
        }

        [HttpPost("update-approvals", Name = "update-approvals/GatherAndStoreApprovals")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult GatherAndStoreApprovals()
        {
            const string requestName = "gather and store approvals";

            try
            {
                _logger.LogInformation($"Received request to {requestName}");

                var request = new ImportApprovalsRequest();
                _taskQueue.QueueBackgroundRequest(request, requestName);

                _logger.LogInformation($"Queued request to {requestName}");

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Request to {requestName} failed");
                return StatusCode(500);
            }
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
