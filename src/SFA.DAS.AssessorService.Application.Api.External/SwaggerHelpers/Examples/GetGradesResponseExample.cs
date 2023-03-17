using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetGradesResponseExample : IExamplesProvider<string[]>
    {
        public string[] GetExamples()
        {
            return new string[] { CertificateGrade.Pass, CertificateGrade.Credit, CertificateGrade.Merit, CertificateGrade.Distinction, CertificateGrade.PassWithExcellence, CertificateGrade.NoGradeAwarded };
        }
    }
}
