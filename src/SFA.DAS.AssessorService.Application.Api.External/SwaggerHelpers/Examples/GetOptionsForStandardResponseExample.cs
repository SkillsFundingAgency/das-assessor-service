using SFA.DAS.AssessorService.Application.Api.External.Models.Standards;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetOptionsForStandardResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new StandardOptions { StandardCode = 6, StandardReference = "ST0156", CourseOption = new[] { "Overhead lines", "Substation fitting", "Underground cables" } };
        }
    }
}
