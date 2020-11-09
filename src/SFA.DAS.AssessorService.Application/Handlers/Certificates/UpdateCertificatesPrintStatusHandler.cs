using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatesPrintStatusHandler : IRequestHandler<UpdateCertificatesPrintStatusRequest, ValidationResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateBatchLogRepository _certificateBatchLogRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateCertificatesPrintStatusHandler> _logger;

        public UpdateCertificatesPrintStatusHandler(ICertificateRepository certificateRepository, 
            ICertificateBatchLogRepository certificateBatchLogRepository,
            IMediator mediator, ILogger<UpdateCertificatesPrintStatusHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _certificateBatchLogRepository = certificateBatchLogRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(UpdateCertificatesPrintStatusRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();
            
            var validatedCertificatePrintStatuses = await Validate(request.CertificatePrintStatuses, validationResult);
            foreach(var validatedCertificatePrintStatus in validatedCertificatePrintStatuses)
            {
                var certificateBatchLog = await _certificateBatchLogRepository.GetCertificateBatchLog(validatedCertificatePrintStatus.CertificateReference, validatedCertificatePrintStatus.BatchNumber);
                if (certificateBatchLog == null)
                {
                    validationResult.Errors.Add(
                      new ValidationErrorDetail("CertificatePrintStatuses", $"Certificate {validatedCertificatePrintStatus.CertificateReference} not printed in batch {validatedCertificatePrintStatus.BatchNumber}.", ValidationStatusCode.NotFound));                    
                }
                else
                {
                    if (validatedCertificatePrintStatus.StatusChangedAt < certificateBatchLog.StatusAt)
                    {
                        validationResult.Errors.Add(
                           new ValidationErrorDetail("StatusChangedDateTime", $"Certificate {validatedCertificatePrintStatus.CertificateReference} new status {validatedCertificatePrintStatus.Status}({validatedCertificatePrintStatus.StatusChangedAt}) should not be earlier than current status {certificateBatchLog.Status}({certificateBatchLog.StatusAt}).", ValidationStatusCode.BadRequest));
                    }
                }

                var certificate = await _certificateRepository.GetCertificate(validatedCertificatePrintStatus.CertificateReference);
                if (certificate == null)
                {
                    validationResult.Errors.Add(
                        new ValidationErrorDetail("CertificatePrintStatuses", $"The certificate reference {validatedCertificatePrintStatus.CertificateReference} was not found.", ValidationStatusCode.NotFound));
                }
                else
                {
                    // use the actual print notification datetime for certificates which currently have a print notification status
                    var certificatePrintStatusAt = CertificateStatus.HasPrintNotificateStatus(certificate.Status)
                        ? certificateBatchLog?.StatusAt
                        : null;

                    // the certificate status should not be overwritten when it has been sent to printer or reprinted, has a more recent 
                    // changed datetime than the actual print notification datetime or when it has been deleted
                    var changesCertificateStatus = validatedCertificatePrintStatus.BatchNumber == certificate.BatchNumber &&
                        validatedCertificatePrintStatus.StatusChangedAt > (certificatePrintStatusAt ?? certificate.LatestChange().Value) &&
                        certificate.Status != CertificateStatus.Deleted;

                    await _certificateRepository.UpdatePrintStatus(
                        certificate,
                        validatedCertificatePrintStatus.BatchNumber, 
                        validatedCertificatePrintStatus.Status, 
                        validatedCertificatePrintStatus.StatusChangedAt,
                        validatedCertificatePrintStatus.ReasonForChange,
                        changesCertificateStatus);

                    _logger.LogInformation($"Certificate reference {validatedCertificatePrintStatus.CertificateReference} set as {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber}");
                }
            }
            
            return validationResult;
        }

        private async Task<List<CertificatePrintStatus>> Validate(List<CertificatePrintStatus> certificatePrintStatuses, ValidationResponse validationResult)
        {
            var invalidPrintStatuses = certificatePrintStatuses
                    .GroupBy(certificatePrintStatus => certificatePrintStatus.Status)
                    .Select(certificatePrintStatus => certificatePrintStatus.Key)
                    .Where(printStatus => !CertificateStatus.HasPrintNotificateStatus(printStatus))
                    .ToList();

            var invalidBatchNumbers = await GetInvalidBatchNumbers(certificatePrintStatuses
                    .GroupBy(certificatePrintStatus => certificatePrintStatus.BatchNumber)
                    .Select(certificatePrintStatus => certificatePrintStatus.Key)
                    .ToList());

            invalidPrintStatuses.ForEach(invalidPrintStatus =>
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail("CertificatePrintStatuses", $"The certificate status {invalidPrintStatus} is not a valid print notification status.", ValidationStatusCode.BadRequest));
            });
            
            invalidBatchNumbers.ForEach(invalidBatchNumber => 
            {
                validationResult.Errors.Add(
                    new ValidationErrorDetail("CertificatePrintStatuses", $"The batch number {invalidBatchNumber} was not found.", ValidationStatusCode.NotFound));
            });

            return certificatePrintStatuses
                .Where(certificatePrintStatus => !invalidBatchNumbers.Contains(certificatePrintStatus.BatchNumber) && !invalidPrintStatuses.Contains(certificatePrintStatus.Status))
                .ToList();
        }

        private async Task<List<int>> GetInvalidBatchNumbers(List<int> batchNumbers)
        {
            var invalidBatchNumbers = new List<int>();

            foreach (var batchNumber in batchNumbers)
            {
                if (await _mediator.Send(new GetForBatchNumberBatchLogRequest { BatchNumber = batchNumber }) == null)
                {
                    invalidBatchNumbers.Add(batchNumber);
                }
            }

            return invalidBatchNumbers;
        }
    }
}