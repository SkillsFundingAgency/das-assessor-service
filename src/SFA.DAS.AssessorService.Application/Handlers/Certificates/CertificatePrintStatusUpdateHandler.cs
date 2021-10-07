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
                            "CertificateReference",
                            $"The certificate reference {validatedCertificatePrintStatus.CertificateReference} was not found.",
                            ValidationStatusCode.NotFound));

                    return validationResult;
                }

                var certificateBatchLog = await _batchLogQueryRepository.GetCertificateBatchLog(validatedCertificatePrintStatus.BatchNumber, validatedCertificatePrintStatus.CertificateReference);
                if (certificateBatchLog == null)
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail(
                            "CertificateReference",
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

                var duplicateUpdate =
                    validatedCertificatePrintStatus.StatusAt == certificateBatchLog.StatusAt &&
                    validatedCertificatePrintStatus.Status == certificateBatchLog.Status &&
                    validatedCertificatePrintStatus.ReasonForChange == certificateBatchLog.ReasonForChange;

                if (!duplicateUpdate)
                {
                    // the actual date of a certificate print status change is not held with the certificate, which holds when
                    // it was notified in it's updatedAt property so fetch the date from the certificate batch log instead
                    var certificatePrintStatusAt = CertificateStatus.HasPrintNotificateStatus(certificate.Status)
                        ? certificateBatchLog?.StatusAt
                        : null;

                    // we are being notified of status changes that took place in the past so the certificate status should only be updated
                    // when the certificate is still in the same batch and no new updates have been made more recently than the status change
                    var updateCertificate = validatedCertificatePrintStatus.BatchNumber == certificate.BatchNumber &&
                        validatedCertificatePrintStatus.StatusAt > (certificatePrintStatusAt ?? certificate.LatestChange().Value) &&
                        certificate.Status != CertificateStatus.Deleted;

                    var updateCertificateBatchLog = validatedCertificatePrintStatus.StatusAt >= certificateBatchLog.StatusAt;

                    await _certificateRepository.UpdatePrintStatus(
                        certificate,
                        certificateBatchLog.BatchNumber,
                        validatedCertificatePrintStatus.Status,
                        validatedCertificatePrintStatus.StatusAt,
                        validatedCertificatePrintStatus.ReasonForChange,
                        updateCertificate,
                        updateCertificateBatchLog);

                    if (updateCertificate)
                    {
                        _logger.LogInformation($"Certificate {validatedCertificatePrintStatus.CertificateReference} updated to status {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber}");
                    }
                    else
                    {
                        _logger.LogInformation($"Certificate {validatedCertificatePrintStatus.CertificateReference} not updated to status {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber} because it was not the latest change");
                    }
                }
                else
                {
                    _logger.LogInformation($"Certificate {validatedCertificatePrintStatus.CertificateReference} not updated to status {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber} because it was a duplicate update");
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
                    new ValidationErrorDetail("BatchNumber", $"The batch number {certificatePrintStatus.BatchNumber} was not found.", ValidationStatusCode.NotFound));
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