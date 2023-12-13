using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    public class BaseController : Controller
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<BaseController> _logger;

        public BaseController(IBackgroundTaskQueue taskQueue, ILogger<BaseController> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected IActionResult QueueBackgroundRequest(IBaseRequest request, string requestName, Action<object, TimeSpan, ILogger<TaskQueueHostedService>> response)
        {
            try
            {
                _logger.LogInformation($"Received request to {requestName}");

                _taskQueue.QueueBackgroundRequest(request, requestName, response);

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
