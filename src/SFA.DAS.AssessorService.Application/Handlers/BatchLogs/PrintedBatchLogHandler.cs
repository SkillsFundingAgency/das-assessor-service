using System;
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

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class PrintedBatchLogHandler : IRequestHandler<PrintedBatchLogRequest, ValidationResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<PrintedBatchLogHandler> _logger;

        public PrintedBatchLogHandler(IBatchLogQueryRepository batchLogQueryRepository, ICertificateRepository certificateRepository, IMediator mediator, ILogger<PrintedBatchLogHandler> logger)
        {             
            _batchLogQueryRepository = batchLogQueryRepository;
            _certificateRepository = certificateRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ValidationResponse> Handle(PrintedBatchLogRequest request, CancellationToken cancellationToken)
        {
            var validationResult = new ValidationResponse();
            var sentToPrinterDate = DateTime.UtcNow;

            if(await _batchLogQueryRepository.GetForBatchNumber(request.BatchNumber) == null)
            {
                validationResult.Errors.Add(new ValidationErrorDetail(nameof(request.BatchNumber), $"The {nameof(request.BatchNumber)} {request.BatchNumber} was not found.", ValidationStatusCode.NotFound));
            }

            if (validationResult.IsValid)
            {
                var certficates = await _certificateRepository.GetCertificatesForBatchLog(request.BatchNumber);
                var result = await _mediator.Send(new UpdateCertificatesPrintStatusRequest
                {
                    CertificatePrintStatuses = certficates.ConvertAll(p => new CertificatePrintStatus
                    {
                        BatchNumber = request.BatchNumber,
                        CertificateReference = p.CertificateReference,
                        Status = CertificateStatus.Printed,
                        StatusChangedAt = request.PrintedAt
                    })
                });

                validationResult.Errors.AddRange(result.Errors);
                _logger.LogInformation($"Batch log {request.BatchNumber} set as {CertificateStatus.Printed}");
            }

            return validationResult;
        }
    } 
}