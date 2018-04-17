using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificateStatusRequest : IRequest
    {
        public List<CertificateStatus> CertificateStatuses { get; set; }
    }
}