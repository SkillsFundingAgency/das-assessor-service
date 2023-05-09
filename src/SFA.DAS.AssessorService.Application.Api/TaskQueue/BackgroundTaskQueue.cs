using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem, string workItemName);

        Task<(Func<CancellationToken, Task> WorkItem, string WorkItemName)> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(Func<CancellationToken, Task> WorkItem, string WorkItemName)> _workItems =
            new ConcurrentQueue<(Func<CancellationToken, Task>, string)>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem, string workItemName)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue((workItem, workItemName));
            _signal.Release();
        }

        public async Task<(Func<CancellationToken, Task> WorkItem, string WorkItemName)> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }

}
