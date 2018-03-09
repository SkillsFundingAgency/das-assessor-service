namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using System.Net.Http;

    public class RestClientResult
    {
        public HttpClient HttpClient { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public string JsonResult { get; set; }
    }
}
