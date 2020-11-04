using System;
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
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class CertificatesPrintStatusUpdateHandler : IRequestHandler<CertificatesPrintStatusUpdateRequest, ValidationResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateBatchLogRepository _certificateBatchLogRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CertificatesPrintStatusUpdateHandler> _logger;

        public CertificatesPrintStatusUpdateHandler(ICertificateRepository certificateRepository, 
            ICertificateBatchLogRepository certificateBatchLogRepository,
            IMediator mediator, ILogger<CertificatesPrintStatusUpdateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _certificateBatchLogRepository = certificateBatchLogRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(CertificatesPrintStatusUpdateRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();
            
            var validatedCertificatePrintStatuses = await Validate(request.CertificatePrintStatusUpdates, validationResult);
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
                    if (validatedCertificatePrintStatus.StatusAt < certificateBatchLog.StatusAt)
                    {
                        validationResult.Errors.Add(
                           new ValidationErrorDetail("StatusChangedDateTime", $"Certificate delivery(StatusChangedAt) datetime {validatedCertificatePrintStatus.StatusAt} earlier than printed(latest date) datetime {certificateBatchLog.LatestChange()}.", ValidationStatusCode.BadRequest));
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
                    // when the certificate batch number is not set then a reprint has been requested but not sent to printer
                    // any print status update would be for a prior batch number and would not update the certificate status
                    var isLatestChange = validatedCertificatePrintStatus.BatchNumber >= (certificate.BatchNumber ?? int.MaxValue) &&
                        validatedCertificatePrintStatus.StatusAt > certificate.LatestChange().Value &&
                        certificate.Status != CertificateStatus.Deleted;

                    await _certificateRepository.UpdatePrintStatus(
                        certificate,
                        validatedCertificatePrintStatus.BatchNumber, 
                        validatedCertificatePrintStatus.Status, 
                        validatedCertificatePrintStatus.StatusAt,
                        validatedCertificatePrintStatus.ReasonForChange,
                        isLatestChange);

                    _logger.LogInformation($"Certificate reference {validatedCertificatePrintStatus.CertificateReference} set as {validatedCertificatePrintStatus.Status} in batch {validatedCertificatePrintStatus.BatchNumber}");
                }
            }
            
            return validationResult;
        }

        private async Task<List<CertificatePrintStatusUpdate>> Validate(List<CertificatePrintStatusUpdate> certificatePrintStatuses, ValidationResponse validationResult)
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
                if (await _mediator.Send(new GetBatchLogRequest { BatchNumber = batchNumber }) == null)
                {
                    invalidBatchNumbers.Add(batchNumber);
                }
            }

            return invalidBatchNumbers;
        }
    }
}