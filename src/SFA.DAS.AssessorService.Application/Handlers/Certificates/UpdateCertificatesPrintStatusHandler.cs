using System.Collections.Generic;
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
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateCertificatesPrintStatusHandler> _logger;

        public UpdateCertificatesPrintStatusHandler(ICertificateRepository certificateRepository, IMediator mediator, ILogger<UpdateCertificatesPrintStatusHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(UpdateCertificatesPrintStatusRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();

            var notFoundBatches = new List<int>();
            var validPrintStatus = new List<string> { CertificateStatus.Printed, CertificateStatus.Delivered, CertificateStatus.NotDelivered };
            
            foreach (var certificatePrintStatus in request.CertificatePrintStatuses)
            {
                if (!notFoundBatches.Contains(certificatePrintStatus.BatchNumber))
                {
                    if(await _mediator.Send(new GetForBatchNumberBatchLogRequest { BatchNumber = certificatePrintStatus.BatchNumber }) == null)
                    {
                        validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.CertificatePrintStatuses), $"The batch number {certificatePrintStatus.BatchNumber} was not found.", ValidationStatusCode.NotFound));
                        notFoundBatches.Add(certificatePrintStatus.BatchNumber);
                    }
                }

                if (!notFoundBatches.Contains(certificatePrintStatus.BatchNumber))
                {
                    var certificate = await _certificateRepository.GetCertificate(certificatePrintStatus.CertificateReference);
                    if (certificate == null)
                    {
                        validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.CertificatePrintStatuses), $"The certificate reference {certificatePrintStatus.CertificateReference} was not found.", ValidationStatusCode.NotFound));
                    }
                    else
                    {
                        var logOnly = certificate.LatestChange().Value > certificatePrintStatus.StatusChangedAt ||
                            certificate.Status == CertificateStatus.Deleted;

                        if (!validPrintStatus.Contains(certificatePrintStatus.Status))
                        {
                            validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.CertificatePrintStatuses), $"The certificate status {certificatePrintStatus.Status} is not a valid print status.", ValidationStatusCode.BadRequest));
                        }
                        else
                        {
                            await _certificateRepository.UpdatePrintStatus(certificate,
                                certificatePrintStatus.BatchNumber, certificatePrintStatus.Status, certificatePrintStatus.StatusChangedAt, logOnly);

                            _logger.LogInformation($"Certificate reference {certificatePrintStatus.CertificateReference} set as {certificatePrintStatus.Status} in batch {certificatePrintStatus.BatchNumber}");
                        }
                    }
                }
            }

            return validationResult;
        }
    } 
}