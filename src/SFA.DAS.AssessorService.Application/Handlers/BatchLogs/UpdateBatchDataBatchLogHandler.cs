using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchDataBatchLogHandler : IRequestHandler<UpdateBatchDataBatchLogRequest, ValidationResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;
        private readonly ILogger<UpdateBatchDataBatchLogHandler> _logger;

        public UpdateBatchDataBatchLogHandler(IBatchLogRepository batchLogRepository, ILogger<UpdateBatchDataBatchLogHandler> logger)
        {
            _batchLogRepository = batchLogRepository;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(UpdateBatchDataBatchLogRequest request, CancellationToken cancellationToken)
        {
            return await _batchLogRepository.UpdateBatchLogBatchWithDataRequest(request.Id, request.BatchData);
        }
    }
}
