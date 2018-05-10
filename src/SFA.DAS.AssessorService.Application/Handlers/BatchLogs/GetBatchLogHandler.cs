using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class GetBatchLogHandler : IRequestHandler<GetLastBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public GetBatchLogHandler(IBatchLogQueryRepository batchLogQueryRepository)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }
        public async Task<BatchLogResponse> Handle(GetLastBatchLogRequest request, CancellationToken cancellationToken)
        {
            var batchLog = await _batchLogQueryRepository.GetLastBatchLog();
            if (batchLog == null)
                return null;

            var batchLogResponse = Mapper.Map<BatchLog, BatchLogResponse>(batchLog);
            return batchLogResponse;
        }
    }
}