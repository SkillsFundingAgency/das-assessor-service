using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Handlers.Certificates
{
    public class GetCertificateHandler : IRequestHandler<GetCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificateHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(GetCertificateRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate<Certificate>(request.CertificateId, request.IncludeLogs);

            if (certificate == null)
                return null;

            if (certificate.PrintRequestedAt == null || string.IsNullOrEmpty(certificate.PrintRequestedBy))
            {
                var printRequestLog = certificate.CertificateLogs?
                    .Where(l => l.Action == CertificateActions.PrintRequest)
                    .OrderByDescending(l => l.EventTime)
                    .FirstOrDefault();

                if (printRequestLog != null)
                {
                    certificate.PrintRequestedAt ??= printRequestLog.EventTime;
                    certificate.PrintRequestedBy ??= printRequestLog.Username;
                }
            }

            return certificate;
        }
    }
}