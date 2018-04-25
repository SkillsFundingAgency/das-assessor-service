using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatesBatchToIndicatePrintedHandler : IRequestHandler<UpdateCertificatesBatchToIndicatePrintedRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificatesBatchToIndicatePrintedHandler> _logger;

        public UpdateCertificatesBatchToIndicatePrintedHandler(ICertificateRepository certificateRepository, ILogger<UpdateCertificatesBatchToIndicatePrintedHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task Handle(UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest, CancellationToken cancellationToken)
        {
            updateCertificatesBatchToIndicatePrintedRequest.CertificateStatuses.ForEach(s =>
            {
                _logger.LogInformation(LoggingConstants.CertificatePrinted);
                _logger.LogInformation($"Certificate with reference of {s.CertificateReference} set as Printed in batch {updateCertificatesBatchToIndicatePrintedRequest.BatchNumber}");
            });

            await _certificateRepository.UpdateStatuses(updateCertificatesBatchToIndicatePrintedRequest);
        }
    } 
}