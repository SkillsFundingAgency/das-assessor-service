using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class SubmitBatchCertificateHandler : IRequestHandler<SubmitBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<SubmitBatchCertificateHandler> _logger;

        public SubmitBatchCertificateHandler(ICertificateRepository certificateRepository, ILogger<SubmitBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(SubmitBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await SubmitCertificate(request);
        }

        private async Task<Certificate> SubmitCertificate(SubmitBatchCertificateRequest request)
        {
            _logger.LogInformation("SubmitCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            _logger.LogInformation("SubmitCertificate Before Update Certificate Status");
            certificate.Status = CertificateStatus.Submitted;

            _logger.LogInformation("SubmitCertificate Before Update Cert in db");
            return await _certificateRepository.Update(certificate, request.Username, CertificateActions.Submit);
        }
    }
}
