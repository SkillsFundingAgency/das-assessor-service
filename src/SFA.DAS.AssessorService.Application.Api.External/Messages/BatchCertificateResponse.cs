using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class BatchCertificateResponse
    {
        public Certificate Certificate { get; set; }

        public CertificateData ProvidedCertificateData { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
