using SFA.DAS.AssessorService.Domain.DTOs;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatesToBePrintedResponse
    {
        public List<CertificateToBePrintedSummary> Certificates { get; set; }
    }
}
