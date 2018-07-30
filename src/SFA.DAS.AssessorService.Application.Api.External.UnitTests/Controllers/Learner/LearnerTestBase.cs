using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using System;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers.Learner
{
    public class LearnerTestBase
    {
        protected Mock<ILogger<LearnerController>> LoggerMock;
        protected ApiClient ApiClientMock;
        protected Mock<HeaderInfo> HeaderInfoMock;
        protected LearnerController ControllerMock;
        protected Mock<FakeHttpMessageHandler> FakeHttpMessageHandlerMock;

        public void Setup()
        {
            BuildLoggerMock();
            BuildApiClientMock();
            BuildHeaderInfoMock();

            ControllerMock = new LearnerController(LoggerMock.Object, HeaderInfoMock.Object, ApiClientMock);
        }

        public void SetHeaderInfo(string username, int ukprn)
        {
            HeaderInfoMock.Object.Username = username;
            HeaderInfoMock.Object.Ukprn = ukprn;
        }

        private void BuildLoggerMock()
        {
            LoggerMock = new Mock<ILogger<LearnerController>>();
        }

        private void BuildApiClientMock()
        {
            FakeHttpMessageHandlerMock = new Mock<FakeHttpMessageHandler> { CallBase = true };

            var httpClient = new HttpClient(FakeHttpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://loopback/")
            };
            var apiClientLogger = new Mock<ILogger<ApiClient>>();
            var tokenService = new Mock<ITokenService>();

            ApiClientMock = new ApiClient(httpClient, apiClientLogger.Object, tokenService.Object);
        }

        private void BuildHeaderInfoMock()
        {
            HeaderInfoMock = new Mock<HeaderInfo>();
            SetHeaderInfo(string.Empty, 0);
        }

        protected void SetFakeHttpMessageHandlerResponse(HttpStatusCode status, object content)
        {
            FakeHttpMessageHandlerMock.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(
                new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(JsonConvert.SerializeObject(content))
                });
        }
    }
}
