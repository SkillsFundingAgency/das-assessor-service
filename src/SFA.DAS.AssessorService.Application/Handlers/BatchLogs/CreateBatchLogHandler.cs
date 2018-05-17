using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class CreateBatchLogHandler : IRequestHandler<CreateBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;

        public CreateBatchLogHandler(IBatchLogRepository batchLogRepository)
        {
            _batchLogRepository = batchLogRepository;
        }

        public async Task<BatchLogResponse> Handle(CreateBatchLogRequest request, CancellationToken cancellationToken)
        {
            var batchLogEntity = Mapper.Map<BatchLog>(request);
            var updatedBatchLogEntity = await _batchLogRepository.Create(batchLogEntity);
            var batchLogResponse = Mapper.Map<BatchLogResponse>(updatedBatchLogEntity);

            return batchLogResponse;
        }
    }
}