using SFA.DAS.AssessorService.Application.Api.External.Models.Standards;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetOptionsForAllStandardResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new[] {
                new StandardOptions { StandardCode = 6, StandardReference = "ST0156", CourseOption = new[] { "Overhead lines", "Substation fitting", "Underground cables" } },
                new StandardOptions { StandardCode = 7, StandardReference = "ST0184", CourseOption = new[] { "Card services", "Corporate/Commercial", "Retail", "Wealth", } },
                new StandardOptions { StandardCode = 314, StandardReference = "ST0018", CourseOption = new[] { "Container based system", "Soil based system" } }
            };
        }
    }
}
