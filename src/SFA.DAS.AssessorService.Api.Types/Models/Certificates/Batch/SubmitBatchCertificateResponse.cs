using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class SubmitBatchCertificateResponse
    {
        public string RequestId { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
