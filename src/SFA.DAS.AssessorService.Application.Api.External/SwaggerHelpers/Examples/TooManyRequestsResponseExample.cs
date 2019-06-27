using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Examples;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class TooManyRequestsResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiResponse((int)HttpStatusCode.Forbidden, "Batch limited to 25 requests");
        }
    }
}
