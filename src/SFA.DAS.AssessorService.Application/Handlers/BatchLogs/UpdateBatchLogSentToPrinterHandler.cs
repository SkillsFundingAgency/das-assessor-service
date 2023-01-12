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
    public class UpdateBatchLogSentToPrinterHandler : IRequestHandler<UpdateBatchLogSentToPrinterRequest, ValidationResponse>
    {
        private readonly IBatchLogRepository _batchLogRepository;

        public UpdateBatchLogSentToPrinterHandler(IBatchLogRepository batchLogRepository)
        {
            _batchLogRepository = batchLogRepository;
        }

        public async Task<ValidationResponse> Handle(UpdateBatchLogSentToPrinterRequest request, CancellationToken cancellationToken)
        {
            var updatedBatchLog = new BatchLog()
            {
                BatchNumber = request.BatchNumber,
                BatchCreated = request.BatchCreated,
                NumberOfCertificates = request.NumberOfCertificates,
                NumberOfCoverLetters = request.NumberOfCoverLetters,
                CertificatesFileName = request.CertificatesFileName,
                FileUploadStartTime = request.FileUploadStartTime,
                FileUploadEndTime = request.FileUploadEndTime,
                BatchData = new BatchData()
                {
                    BatchNumber = request.BatchNumber
                }
            };

            return await _batchLogRepository.UpdateBatchLogSentToPrinter(updatedBatchLog);
        }
    }
}
