using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateStatusRequest : IRequest
    {
        public string CertificateReference { get; set; }
        public string Status { get; set; }
    }
}