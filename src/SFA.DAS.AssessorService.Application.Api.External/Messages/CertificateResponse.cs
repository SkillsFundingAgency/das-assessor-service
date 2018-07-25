using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class CertificateResponse
    {
            public long Uln { get; set; }
            public string LastName { get; set; }
            public int StdCode { get; set; }

            public CertificateData CertificateData { get; set; }
            public Certificate Certificate { get; set; }

            public string Status { get; set; }
            public List<string> ValidationErrors { get; set; }
    }
}
