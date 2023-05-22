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
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/providers/")]
    [ValidateBadRequest]
    public class ProvidersController : Controller
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ProvidersController> _logger;

        public ProvidersController(IBackgroundTaskQueue taskQueue, ILogger<ProvidersController> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        [HttpPost("refresh-providers", Name = "update-providers/RefreshProvidersCache")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult RefreshProvidersCache()
        {
            const string requestName = "refresh providers cache";

            try
            {
                _logger.LogInformation($"Received request to {requestName}");

                var request = new UpdateProvidersCacheRequest()
                {
                    UpdateType = ProvidersCacheUpdateType.RefreshExistingProviders
                };

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
    }
}
