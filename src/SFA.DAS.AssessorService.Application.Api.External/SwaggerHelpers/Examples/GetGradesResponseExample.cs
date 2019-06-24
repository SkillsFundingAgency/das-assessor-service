using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetGradesResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new string[] { "Pass", "Credit", "Merit", "Distinction", "Pass with excellence", "No grade awarded" };
        }
    }
}
