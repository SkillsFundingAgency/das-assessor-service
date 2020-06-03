using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateBatchLogSentToPrinterHandler : IRequestHandler<UpdateBatchLogSentToPrinterRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateBatchLogSentToPrinterHandler> _logger;

        public UpdateBatchLogSentToPrinterHandler(ICertificateRepository certificateRepository, ILogger<UpdateBatchLogSentToPrinterHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateBatchLogSentToPrinterRequest request, CancellationToken cancellationToken)
        {
            var sentToPrinterDate = DateTime.UtcNow;

            foreach(var certificateReference in request.CertificateReferences)
            {
                _logger.LogInformation($"Certificate reference {certificateReference} set as {CertificateStatus.SentToPrinter} in batch {request.BatchNumber}");

                await _certificateRepository.UpdateSentToPrinter(certificateReference, request.BatchNumber, sentToPrinterDate);
            }

            return Unit.Value;
        }
    } 
}