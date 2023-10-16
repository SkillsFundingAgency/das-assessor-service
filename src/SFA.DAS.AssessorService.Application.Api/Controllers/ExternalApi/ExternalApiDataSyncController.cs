using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.DataSync;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/externalapidatasync")]
    [ValidateBadRequest]
    public class ExternalApiDataSyncController : BaseController
    {
        public ExternalApiDataSyncController(IBackgroundTaskQueue taskQueue, ILogger<ExternalApiDataSyncController> logger)
            : base(taskQueue, logger)
        {
        }

        [HttpPost("rebuild-sandbox", Name = "RebuildExternalApiSandbox")]
        [SwaggerResponse((int)HttpStatusCode.Accepted)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public IActionResult RebuildExternalApiSandbox()
        {
            return QueueBackgroundRequest(new RebuildExternalApiSandboxRequest(), "rebuild external api sandbox");
        }
    }
}
