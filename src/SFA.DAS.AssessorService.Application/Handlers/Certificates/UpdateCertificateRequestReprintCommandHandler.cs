using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateRequestReprintCommandHandler : IRequestHandler<UpdateCertificateRequestReprintCommand, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public UpdateCertificateRequestReprintCommandHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequestReprintCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate<Certificate>(request.CertificateId);
            if (certificate == null)
                throw new NotFoundException();

            if (certificate.Status != CertificateStatus.Reprint && CertificateStatus.CanRequestReprintCertificate(certificate.Status))
            {
                certificate.Status = CertificateStatus.Reprint;
                await _certificateRepository.UpdateStandardCertificate(certificate, request.Username, CertificateActions.Reprint);
            }

            return certificate;
        }
    }
}