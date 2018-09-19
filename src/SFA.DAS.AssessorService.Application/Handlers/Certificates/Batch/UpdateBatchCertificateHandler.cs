using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates.Batch
{
    public class UpdateBatchCertificateHandler : IRequestHandler<UpdateBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateBatchCertificateHandler> _logger;

        public UpdateBatchCertificateHandler(ICertificateRepository certificateRepository, ILogger<UpdateBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(UpdateBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await UpdateCertificate(request);
        }

        private async Task<Certificate> UpdateCertificate(UpdateBatchCertificateRequest request)
        {
            _logger.LogInformation("UpdateCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            _logger.LogInformation("UpdateCertificate Before Update CertificateData");

            certificate.CertificateData = JsonConvert.SerializeObject(request.CertificateData);

            if(certificate.Status == CertificateStatus.Deleted)
            {
                certificate.Status = CertificateStatus.Draft;
            }

            _logger.LogInformation("UpdateCertificate Before Update Cert in db");
            await _certificateRepository.Update(certificate, request.Username, null);

            return certificate;
        }
    }
}
