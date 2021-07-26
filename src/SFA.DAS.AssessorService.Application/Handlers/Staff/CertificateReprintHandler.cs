using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class CertificateReprintHandler : IRequestHandler<StaffCertificateDuplicateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public CertificateReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(StaffCertificateDuplicateRequest request,
            CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.Id);
            if (CertificateStatus.CanRequestDuplicateCertificate(certificate.Status))
            {
                certificate.Status = CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username, action: CertificateActions.Reprint);
            }

            return certificate;
        }
    }
}