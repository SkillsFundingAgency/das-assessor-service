using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/providers/")]
    [ValidateBadRequest]
    public class ProvidersController : BaseController
    {
        public ProvidersController(IBackgroundTaskQueue taskQueue, ILogger<ProvidersController> logger)
            : base(taskQueue, logger)
        {
        }

        [HttpPost("refresh-providers", Name = "update-providers/RefreshProvidersCache")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult RefreshProvidersCache()
        {
            var requestName = "refresh providers cache";
            return QueueBackgroundRequest(new UpdateProvidersCacheRequest() { UpdateType = ProvidersCacheUpdateType.RefreshExistingProviders }, 
                requestName, (response, duration, log) =>
                {
                    log.LogInformation($"Completed request to {requestName} in {duration.ToReadableString()}");
                });
        }
    }
}
