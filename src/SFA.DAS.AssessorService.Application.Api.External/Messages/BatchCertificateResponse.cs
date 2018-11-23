using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class BatchCertificateResponse
    {
        public string RequestId { get; set; }

        public Certificate Certificate { get; set; }

        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}
