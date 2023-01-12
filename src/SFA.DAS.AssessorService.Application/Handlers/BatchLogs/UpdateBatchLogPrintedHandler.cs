using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchLogPrintedHandler : IRequestHandler<UpdateBatchLogPrintedRequest, ValidationResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;

        public UpdateBatchLogPrintedHandler(IBatchLogRepository batchLogRepository)
        {
            _batchLogRepository = batchLogRepository;
        }

        public async Task<ValidationResponse> Handle(UpdateBatchLogPrintedRequest request, CancellationToken cancellationToken)
        {
            var updatedBatchLog = new BatchLog
            {
                BatchNumber = request.BatchNumber,
                BatchData = new BatchData()
                {
                    BatchNumber = request.BatchNumber,
                    BatchDate = request.BatchDate,
                    PostalContactCount = request.PostalContactCount,
                    TotalCertificateCount = request.TotalCertificateCount,
                    PrintedDate = request.PrintedDate,
                    DateOfResponse = request.DateOfResponse
                }
            };

            return await _batchLogRepository.UpdateBatchLogPrinted(updatedBatchLog);
        }
    }
}
