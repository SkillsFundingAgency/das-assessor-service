using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
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

            if (certificate.Status == Domain.Consts.CertificateStatus.Printed)
            {
                certificate.Status = Domain.Consts.CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username,
                    action: Domain.Consts.CertificateActions.Reprint);
            }

            return certificate;
        }
    }
}