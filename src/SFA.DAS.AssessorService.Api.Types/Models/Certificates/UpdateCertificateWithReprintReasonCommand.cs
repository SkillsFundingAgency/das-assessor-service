using MediatR;
using SFA.DAS.AssessorService.Api.Types.Enums;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateWithReprintReasonCommand : IRequest
    {
        public string CertificateReference { get; set; }
        public string Username { get; set; }
        public string IncidentNumber { get; set; }
        public ReprintReasons? Reasons { get; set; }
        public string OtherReason { get; set; }
    }
}
