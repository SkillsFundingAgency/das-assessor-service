using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class CreateBatchLogHandler : BaseHandler, IRequestHandler<CreateBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public CreateBatchLogHandler(IBatchLogRepository batchLogRepository, IBatchLogQueryRepository batchLogQueryRepository, IMapper mapper)
            :base(mapper)
        {
            _batchLogRepository = batchLogRepository;
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<BatchLogResponse> Handle(CreateBatchLogRequest request, CancellationToken cancellationToken)
        {
            var lastBatchLog = await _batchLogQueryRepository.GetLastBatchLog();

            var batchLog = new BatchLog()
            {
                Period = DateTime.UtcNow.ToString("MMyy"),
                ScheduledDate = request.ScheduledDate,
                BatchNumber = lastBatchLog.BatchNumber + 1
            };

            batchLog = await _batchLogRepository.Create(batchLog);
            return _mapper.Map<BatchLogResponse>(batchLog);
        }
    }
}