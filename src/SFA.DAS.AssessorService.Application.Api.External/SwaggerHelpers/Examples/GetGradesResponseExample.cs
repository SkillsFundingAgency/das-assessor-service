using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetGradesResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.NoGradeAwarded };
        }
    }
}
