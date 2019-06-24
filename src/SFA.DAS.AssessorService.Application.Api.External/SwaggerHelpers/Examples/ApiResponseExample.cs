using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Examples;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class ApiResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiResponse((int)HttpStatusCode.Forbidden, "ULN, FamilyName and Standard not found");
        }
    }
}
