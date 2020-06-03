using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatesPrintStatusHandler : IRequestHandler<UpdateCertificatesPrintStatusRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificatesPrintStatusHandler> _logger;

        public UpdateCertificatesPrintStatusHandler(ICertificateRepository certificateRepository, ILogger<UpdateCertificatesPrintStatusHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCertificatesPrintStatusRequest request, CancellationToken cancellationToken)
        {
            foreach(var certificatePrintStatus in request.CertificatePrintStatuses)
            {
                _logger.LogInformation($"Certificate reference {certificatePrintStatus.CertificateReference} set as {certificatePrintStatus.Status} in batch {certificatePrintStatus.BatchNumber}");

                var certificate = await _certificateRepository.GetCertificate(certificatePrintStatus.CertificateReference);

                var logOnly = certificate.LatestChange().Value > certificatePrintStatus.StatusChangedAt ||
                    certificate.Status == CertificateStatus.Deleted;

                await _certificateRepository.UpdatePrintStatus(certificatePrintStatus.CertificateReference,
                    certificatePrintStatus.BatchNumber, certificatePrintStatus.Status, certificatePrintStatus.StatusChangedAt, logOnly);
            }
            
            return Unit.Value;
        }
    } 
}