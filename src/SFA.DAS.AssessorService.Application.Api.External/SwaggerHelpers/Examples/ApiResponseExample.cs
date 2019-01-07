using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using Swashbuckle.AspNetCore.Examples;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class ApiResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiResponse((int)HttpStatusCode.Forbidden, "Cannot find apprentice with the specified Uln, FamilyName & StandardCode");
        }
    }
}
