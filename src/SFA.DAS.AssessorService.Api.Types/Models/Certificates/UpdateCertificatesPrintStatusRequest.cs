using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificatesPrintStatusRequest : IRequest
    {
        public List<CertificatePrintStatus> CertificatePrintStatuses { get; set; }
    }
}