using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public class TaskQueueHostedService : BackgroundService
    {
        private readonly ILogger<TaskQueueHostedService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public TaskQueueHostedService(IBackgroundTaskQueue taskQueue,
            ILogger<TaskQueueHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Queue Hosted Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var (workItem, workItemName) = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                       "Error occurred executing {WorkItemName}.", workItemName);
                }
            }

            _logger.LogInformation("Task Queue Hosted Service is stopping.");
        }
    }
}