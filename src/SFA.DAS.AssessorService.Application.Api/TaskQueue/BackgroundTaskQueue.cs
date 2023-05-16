using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundRequest(IRequest request, string requestName);

        Task<(IRequest request, string RequestName)> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(IRequest Request, string RequestName)> _requests = new ConcurrentQueue<(IRequest, string)>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundRequest(IRequest request, string requestName)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _requests.Enqueue((request, requestName));
            _signal.Release();
        }

        public async Task<(IRequest request, string RequestName)> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _requests.TryDequeue(out var request);

            return request;
        }
    }

}
