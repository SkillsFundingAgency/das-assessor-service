using System;
using FizzWare.NBuilder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.UnitTests.MockedObjects
{
    public class MockedCertificateApiClient
    {
        public static CertificateApiClient Setup(Certificate certificate, Mock<ILogger<CertificateApiClient>> apiClientLoggerMock)
        {
            var webConfigMock = new Mock<IWebConfiguration>();
            var hostMock = new Mock<IHostEnvironment>();
            hostMock
                .Setup(m => m.EnvironmentName)
                .Returns(Environments.Development);
            var tokenServiceMock = new TokenService(webConfigMock.Object, hostMock.Object, false);

            var options = Builder<Option>.CreateListOfSize(10)
                .Build();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{certificate.Id}?includeLogs={false}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/{certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/options/?stdCode={93}")
                .Respond("application/json", JsonConvert.SerializeObject(options));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/options/?stdCode={1}")
                .Respond("application/json", JsonConvert.SerializeObject(options));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/start")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));
           
            mockHttp
                .When(System.Net.Http.HttpMethod.Put, "http://localhost:59022/api/v1/certificates/update")
                .Respond(System.Net.HttpStatusCode.OK, "application/json", "{'status' : 'OK'}");

            var apiClient = new CertificateApiClient(client, tokenServiceMock, apiClientLoggerMock.Object);

            return apiClient;
        }
    }
}
