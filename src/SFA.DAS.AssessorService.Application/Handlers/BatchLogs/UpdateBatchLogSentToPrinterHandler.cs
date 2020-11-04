using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class UpdateBatchLogSentToPrinterHandler : IRequestHandler<UpdateBatchLogSentToPrinterRequest, ValidationResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;

        public UpdateBatchLogSentToPrinterHandler(IBatchLogRepository batchLogRepository)
        {
            _batchLogRepository = batchLogRepository;
        }

        public async Task<ValidationResponse> Handle(UpdateBatchLogSentToPrinterRequest request, CancellationToken cancellationToken)
        {
            return await _batchLogRepository.UpdateBatchLogSentToPrinter(
                request.BatchNumber, 
                request.BatchCreated, 
                request.NumberOfCertificates, 
                request.NumberOfCoverLetters,
                request.CertificatesFileName,
                request.FileUploadStartTime, 
                request.FileUploadEndTime,
                new BatchData()
                {
                    BatchNumber = request.BatchNumber
                });
        }
    }
}
