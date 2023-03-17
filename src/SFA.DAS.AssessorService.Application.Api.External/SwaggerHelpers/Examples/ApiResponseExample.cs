using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class ApiResponseExample : IExamplesProvider<ApiResponse>
    {
        public ApiResponse GetExamples()
        {
            return new ApiResponse((int)HttpStatusCode.Forbidden, "ULN, FamilyName and Standard not found");
        }
    }
}
