using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class BatchCertificateResponse
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }

        public CertificateData ProvidedCertificateData { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
