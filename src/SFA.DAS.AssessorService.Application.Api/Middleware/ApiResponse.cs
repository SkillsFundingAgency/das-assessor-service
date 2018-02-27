namespace SFA.DAS.AssessorService.Application.Api.Middleware
{
    using Newtonsoft.Json;

    public class ApiResponse
    {
        public int StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
