using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response
{
    public class GetCertificateLogsResponse
    {
        public IEnumerable<CertificateLog> CertificateLogs { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
