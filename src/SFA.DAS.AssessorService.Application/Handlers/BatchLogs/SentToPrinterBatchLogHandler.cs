using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class SentToPrinterBatchLogHandler : IRequestHandler<SentToPrinterBatchLogRequest, ValidationResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SentToPrinterBatchLogHandler> _logger;

        public SentToPrinterBatchLogHandler(ICertificateRepository certificateRepository, IBatchLogQueryRepository batchLogQueryRepository, ILogger<SentToPrinterBatchLogHandler> logger)
        {             
            _certificateRepository = certificateRepository;
            _batchLogQueryRepository = batchLogQueryRepository;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(SentToPrinterBatchLogRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();
            var sentToPrinterDate = DateTime.UtcNow;

            if(await _batchLogQueryRepository.Get(request.BatchNumber) == null)
            {
                validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.BatchNumber), $"The {nameof(request.BatchNumber)} {request.BatchNumber} was not found.", ValidationStatusCode.NotFound));
            }

            if (validationResult.IsValid)
            {
                foreach (var certificateReference in request.CertificateReferences)
                {
                    var certificate = await _certificateRepository.GetCertificate(certificateReference);
                    if (certificate == null)
                    {
                        validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.CertificateReferences), $"The certificate reference {certificateReference} was not found.", ValidationStatusCode.NotFound));
                    }
                    else
                    {
                        await _certificateRepository.UpdateSentToPrinter(certificate, request.BatchNumber, sentToPrinterDate);
                        _logger.LogInformation($"Certificate reference {certificateReference} set as {CertificateStatus.SentToPrinter} in batch {request.BatchNumber}");
                    }
                }
            }

            return validationResult;
        }
    } 
}