using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class GetForBatchNumberBatchLogHandler : IRequestHandler<GetForBatchNumberBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public GetForBatchNumberBatchLogHandler(IBatchLogQueryRepository batchLogQueryRepository)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<BatchLogResponse> Handle(GetForBatchNumberBatchLogRequest request, CancellationToken cancellationToken)
        {
            var batchLog = await _batchLogQueryRepository.GetForBatchNumber(request.BatchNumber);
            return Mapper.Map<BatchLog, BatchLogResponse>(batchLog);
        }
    }
}
