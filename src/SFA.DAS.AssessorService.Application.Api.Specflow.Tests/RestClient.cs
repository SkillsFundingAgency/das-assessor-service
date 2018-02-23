namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using System.Net.Http;

    public class RestClient
    {
        public HttpClient HttpClient { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public string Result { get; set; }
    }
}
