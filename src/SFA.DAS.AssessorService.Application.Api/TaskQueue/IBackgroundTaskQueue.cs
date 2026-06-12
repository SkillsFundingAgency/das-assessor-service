using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using System;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundRequest(IBaseRequest request, string requestName, Action<object, TimeSpan, ILogger<TaskQueueHostedService>> response);

        Task<(IBaseRequest Request, string RequestName, Action<object, TimeSpan, ILogger<TaskQueueHostedService>> Response)> DequeueAsync(CancellationToken cancellationToken);
    }
}
