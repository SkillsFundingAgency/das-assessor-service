using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundRequest(IBaseRequest request, string requestName, string responseMessage);

        Task<(IBaseRequest request, string RequestName, string ResponseMessage)> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(IBaseRequest Request, string RequestName, string ResponseMessage)> _requests = new ConcurrentQueue<(IBaseRequest, string, string)>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundRequest(IBaseRequest request, string requestName, string responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _requests.Enqueue((request, requestName, responseMessage));
            _signal.Release();
        }

        public async Task<(IBaseRequest request, string RequestName, string ResponseMessage)> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _requests.TryDequeue(out var request);

            return request;
        }
    }

}
