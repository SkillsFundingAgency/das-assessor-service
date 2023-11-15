using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Api.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundRequest(IBaseRequest request, string requestName, string responseMessage);

        Task<(IBaseRequest request, string RequestName, string ResponseMessage)> DequeueAsync(CancellationToken cancellationToken);
    }

}
