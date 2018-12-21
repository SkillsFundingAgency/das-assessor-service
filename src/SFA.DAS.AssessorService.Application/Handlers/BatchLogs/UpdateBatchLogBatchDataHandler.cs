using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchLogBatchDataHandler : IRequestHandler<UpdateBatchLogBatchDataRequest, ValidationResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;
        private readonly ILogger<UpdateBatchLogBatchDataHandler> _logger;

        public UpdateBatchLogBatchDataHandler(IBatchLogRepository batchLogRepository, ILogger<UpdateBatchLogBatchDataHandler> logger)
        {
            _batchLogRepository = batchLogRepository;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(UpdateBatchLogBatchDataRequest request, CancellationToken cancellationToken)
        {
            return await _batchLogRepository.UpdateBatchLogBatchWithDataRequest(request.Id, request.BatchData);
        }
    }
}
