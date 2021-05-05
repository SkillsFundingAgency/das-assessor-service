using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class CertificatePrintStatusUpdateHandler : IRequestHandler<CertificatePrintStatusUpdateRequest, ValidationResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CertificatePrintStatusUpdateHandler> _logger;

        public CertificatePrintStatusUpdateHandler(ICertificateRepository certificateRepository, 
            IBatchLogQueryRepository batchLogQueryRepository,
            IMediator mediator, ILogger<CertificatePrintStatusUpdateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _batchLogQueryRepository = batchLogQueryRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(CertificatePrintStatusUpdateRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();

            var validatedCertificatePrintStatus = await Validate(request, validationResult);
            if (validatedCertificatePrintStatus != null)
            {
                var certificate = await _certificateRepository.GetCertificate(validatedCertificatePrintStatus.CertificateReference);
                if (certificate == null)
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail(
                            "CertificatePrintStatuses",
                            $"The certificate reference {validatedCertificatePrintStatus.CertificateReference} was not found.",
                            ValidationStatusCode.NotFound));

                    return validationResult;
                }

                var certificateBatchLog = await _batchLogQueryRepository.GetCertificateBatchLog(validatedCertificatePrintStatus.BatchNumber, validatedCertificatePrintStatus.CertificateReference);
                if (certificateBatchLog == null)
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail(
                            "CertificatePrintStatuses",
                            $"Certificate {validatedCertificatePrintStatus.CertificateReference} not printed in batch {validatedCertificatePrintStatus.BatchNumber}.",
                            ValidationStatusCode.NotFound));

                    return validationResult;
                }

                if (validatedCertificatePrintStatus.StatusAt < certificateBatchLog.StatusAt)
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail(
                            "StatusChangedDateTime", 
                            $"Certificate {validatedCertificatePrintStatus.CertificateReference} {validatedCertificatePrintStatus.Status} at {validatedCertificatePrintStatus.StatusAt} is earlier than {certificateBatchLog.Status} at {certificateBatchLog.StatusAt}.", 
                            ValidationStatusCode.Warning));
                }

                // use the actual print notification datetime for certificates which currently have a print notification status
                var certificatePrintStatusAt = CertificateStatus.HasPrintNotificateStatus(certificate.Status)
                    ? certificateBatchLog?.StatusAt
                    : null;

                // the certificate status should not be overwritten when it has been sent to printer or reprinted, has a more recent 
                // changed datetime than the actual print notification datetime or when it has been deleted
                var isLatestChange = validatedCertificatePrintStatus.BatchNumber == certificate.BatchNumber &&
                    validatedCertificatePrintStatus.StatusAt > (certificatePrintStatusAt ?? certificate.LatestChange().Value) &&
                    certificate.Status != CertificateStatus.Deleted;

                await _certificateRepository.UpdatePrintStatus(
                    certificate,
                    validatedCertificatePrintStatus.BatchNumber,
                    validatedCertificatePrintStatus.Status,
                    validatedCertificatePrintStatus.StatusAt,
                    validatedCertificatePrintStatus.ReasonForChange,
                    isLatestChange);

                if(isLatestChange)
                {
                    _logger.LogInformation($"Certificate {validatedCertificatePrintStatus.CertificateReference} updated to status {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber}");
                }
                else
                {
                    _logger.LogInformation($"Certificate {validatedCertificatePrintStatus.CertificateReference} not updated to status {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber} because it was not the latest change");
                }
            }

            return validationResult;
        }

        private async Task<CertificatePrintStatusUpdate> Validate(CertificatePrintStatusUpdate certificatePrintStatus, ValidationResponse validationResult)
        {
            if(!CertificateStatus.HasPrintNotificateStatus(certificatePrintStatus.Status))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail("CertificatePrintStatuses", $"The certificate status {certificatePrintStatus.Status} is not a valid print notification status.", ValidationStatusCode.BadRequest));
            }
            
            if(!await IsValidBatchNumber(certificatePrintStatus.BatchNumber))
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail("CertificatePrintStatuses", $"The batch number {certificatePrintStatus.BatchNumber} was not found.", ValidationStatusCode.NotFound));
            }

            return validationResult.Errors.Count == 0
                ? certificatePrintStatus
                : null;
        }

        private async Task<bool> IsValidBatchNumber(int batchNumber)
        {
            if (await _mediator.Send(new GetBatchLogRequest { BatchNumber = batchNumber }) == null)
            {
                return false;
            }

            return true;
        }
    }
}