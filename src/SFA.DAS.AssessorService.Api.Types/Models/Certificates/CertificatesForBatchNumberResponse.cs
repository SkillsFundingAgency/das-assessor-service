using SFA.DAS.AssessorService.Domain.DTOs;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatesForBatchNumberResponse
    {
        public List<CertificatePrintSummaryBase> Certificates { get; set; }
    }
}
