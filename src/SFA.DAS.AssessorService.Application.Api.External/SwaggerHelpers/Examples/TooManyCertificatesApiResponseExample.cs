using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using Swashbuckle.AspNetCore.Examples;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class TooManyCertificatesApiResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiResponse((int)HttpStatusCode.Forbidden, "There are too many certificates specified within the request. Please specify no more than 25");
        }
    }
}
