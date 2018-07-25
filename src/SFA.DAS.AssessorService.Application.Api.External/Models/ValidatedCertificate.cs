using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Models
{
    public class ValidatedCertificate
    {
        public long Uln { get; set; }
        public string LastName { get; set; }
        public int StdCode { get; set; }

        public Domain.JsonData.CertificateData CertificateData { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
        public bool IsVaild => ValidationErrors.Count == 0;
    }
}
