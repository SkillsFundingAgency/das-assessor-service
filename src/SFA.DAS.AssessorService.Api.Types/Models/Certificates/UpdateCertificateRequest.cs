using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateRequest : IRequest<Certificate>
    {
        public Certificate Certificate { get; }

        public UpdateCertificateRequest(Certificate certificate)
        {
            Certificate = certificate;
        }
    }
}