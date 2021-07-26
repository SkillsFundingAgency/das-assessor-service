using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class RequestReprintHandler : IRequestHandler<CertificateReprintRequest>
    {
        private readonly ICertificateRepository _certificateRepository;

        public RequestReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Unit> Handle(CertificateReprintRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.CertificateReference, request.LastName, request.AchievementDate);
            if (certificate == null)
                throw new NotFound();

            if (certificate.Status == CertificateStatus.Reprint)
            {
                return Unit.Value;
            }

            if (CertificateStatus.CanRequestDuplicateCertificate(certificate.Status))
            {
                certificate.Status = CertificateStatus.Reprint;
                await _certificateRepository.Update(certificate, request.Username, CertificateActions.Reprint);
            }
            else
            {
                throw new NotFound();
            }

            return Unit.Value;
        }
    }
}