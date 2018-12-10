using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class SubmitCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<SubmitCertificate>
            {
                new SubmitCertificate
                {
                    RequestId = "1",
                    CertificateReference = "09876543",
                    FamilyName = "Smith",
                    StandardCode = 1,
                    Uln = 1234567890
                },
                new SubmitCertificate
                {
                    RequestId = "2",
                    CertificateReference = "99999999",
                    StandardCode = 99,
                    Uln = 9999999999
                }
            };
        }
    }
}
