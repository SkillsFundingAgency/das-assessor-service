using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Api.Middleware
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public int StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }
    }
}