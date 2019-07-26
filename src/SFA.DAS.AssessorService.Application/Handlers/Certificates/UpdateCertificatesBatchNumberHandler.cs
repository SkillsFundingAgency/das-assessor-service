using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatesBatchNumberHandler : IRequestHandler<UpdateCertificatesBatchNumberRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificatesBatchToIndicatePrintedHandler> _logger;

        public UpdateCertificatesBatchNumberHandler(ICertificateRepository certificateRepository, ILogger<UpdateCertificatesBatchToIndicatePrintedHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }
        public async Task Handle(UpdateCertificatesBatchNumberRequest updateCertificatesBatchNumberRequest, CancellationToken cancellationToken)
        {
            updateCertificatesBatchNumberRequest.CertificateReference.ForEach(s =>
            {
                _logger.LogInformation($"Certificate for certificate  reference of {s}, is being updated with batch number {updateCertificatesBatchNumberRequest.BatchNumber}");
            });

            await _certificateRepository.UpdateCertificateWithBatchNumber(updateCertificatesBatchNumberRequest);
        }
    }
}
