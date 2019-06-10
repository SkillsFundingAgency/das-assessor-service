using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using Swashbuckle.AspNetCore.Examples;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class SubmitCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<SubmitCertificateRequest>
            {
                new SubmitCertificateRequest
                {
                    RequestId = "1",
                    CertificateReference = "09876543",
                    FamilyName = "Smith",
                    StandardReference = "ST0001",
                    Uln = 1234567890
                },
                new SubmitCertificateRequest
                {
                    RequestId = "2",
                    CertificateReference = "99999999",
                    FamilyName = "Hamilton",
                    StandardCode = 99,
                    Uln = 9999999999
                },
                new SubmitCertificateRequest
                {
                    RequestId = "3",
                    CertificateReference = "55555555",
                    FamilyName = "Unknown",
                    StandardCode = 555,
                    StandardReference = "ST0555",
                    Uln = 5555555555
                },
            };
        }
    }
}
