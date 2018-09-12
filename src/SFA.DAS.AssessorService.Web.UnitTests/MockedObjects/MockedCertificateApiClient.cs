using System;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Web.UnitTests.MockedObjects
{
    public class MockedCertificateApiClient
    {
        public static CertificateApiClient Setup(Certificate certificate, Mock<ILogger<CertificateApiClient>> apiClientLoggerMock)
        {           
            var tokenServiceMock = new Mock<ITokenService>();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/{certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            var apiClient = new CertificateApiClient(client, tokenServiceMock.Object, apiClientLoggerMock.Object);

            return apiClient;
        }
    }
}
