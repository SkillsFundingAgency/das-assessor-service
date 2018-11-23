using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class SubmitCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SubmitCertificate
            {
                RequestId = "1",
                CertificateReference = "09876543",
                FamilyName = "Smith",
                StandardCode = 1,
                Uln = 1234567890
            };
        }
    }
}
