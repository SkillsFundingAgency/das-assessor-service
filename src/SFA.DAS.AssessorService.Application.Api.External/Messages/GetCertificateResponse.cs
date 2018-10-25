using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class GetCertificateResponse
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }
        public string CertificateReference { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
