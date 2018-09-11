using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class CertificateReprintHandler : IRequestHandler<StaffCertificateDuplicateRequest, CertificateReprintResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public CertificateReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<CertificateReprintResponse> Handle(StaffCertificateDuplicateRequest request,
            CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.Id);

            if (certificate.Status == Domain.Consts.CertificateStatus.Printed)
            {
                certificate.Status = Domain.Consts.CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username,
                    action: Domain.Consts.CertificateActions.Reprint);
            }

            var staffUiReprintResponse = new CertificateReprintResponse
            {
                Certificate = certificate
            };

            return staffUiReprintResponse;
        }
    }
}