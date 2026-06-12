using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class GetLastBatchLogHandler : BaseHandler, IRequestHandler<GetLastBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public GetLastBatchLogHandler(IBatchLogQueryRepository batchLogQueryRepository, IMapper mapper)
            :base(mapper)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<BatchLogResponse> Handle(GetLastBatchLogRequest request, CancellationToken cancellationToken)
        {
            var batchLog = await _batchLogQueryRepository.GetLastBatchLog();
            if (batchLog == null)
                return null;

            var batchLogResponse = _mapper.Map<BatchLog, BatchLogResponse>(batchLog);
            return batchLogResponse;
        }
    }
}