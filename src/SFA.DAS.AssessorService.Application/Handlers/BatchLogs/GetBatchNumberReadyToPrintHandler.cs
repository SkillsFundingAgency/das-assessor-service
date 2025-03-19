using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Handlers.BatchLogs
{
    public class GetBatchNumberReadyToPrintHandler : IRequestHandler<GetBatchNumberReadyToPrintRequest, int?>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public GetBatchNumberReadyToPrintHandler(IBatchLogQueryRepository batchLogQueryRepository)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<int?> Handle(GetBatchNumberReadyToPrintRequest request, CancellationToken cancellationToken)
        {
            return await _batchLogQueryRepository.GetBatchNumberReadyToPrint();
        }
    }
}
