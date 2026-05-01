using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesUlnResponse
    {
        public List<ApprenticeCertificateSummary> Certificates { get; set; }
    }
}
