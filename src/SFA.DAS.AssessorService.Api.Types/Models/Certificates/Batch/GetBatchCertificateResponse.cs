using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class GetBatchCertificateResponse
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string Standard { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
