using System;
using System.Collections.Generic;
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
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.MockedObjects
{
    public class MockedCertificateApiClient
    {
        public static CertificateApiClient Setup(Certificate certificate, Mock<ILogger<CertificateApiClient>> apiClientLoggerMock)
        {
            var webConfigMock = new Mock<IWebConfiguration>();
            var hostMock = new Mock<IHostingEnvironment>();
            hostMock
                .Setup(m => m.EnvironmentName)
                .Returns(EnvironmentName.Development);
            var tokenServiceMock = new TokenService(webConfigMock.Object,hostMock.Object);

            var options = Builder<Option>.CreateListOfSize(10)
                .Build();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/{certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(certificate));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/options/?stdCode={93}")
                .Respond("application/json", JsonConvert.SerializeObject(options));            

            var certificateFirstNameViewModel = new CertificateFirstNameViewModel
            {
                Id = new Guid("1f120837-72d5-40eb-a785-b3936210d47a"),
                FullName = "James Corley",
                FirstName = "James",
                FamilyName = "Corley",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true
            };
            
            mockHttp
                .When(System.Net.Http.HttpMethod.Put, "http://localhost:59022/api/v1/certificates/update")
                .Respond(System.Net.HttpStatusCode.OK, "application/json", "{'status' : 'OK'}");

            var apiClient = new CertificateApiClient(client, tokenServiceMock, apiClientLoggerMock.Object);

            return apiClient;
        }
    }
}
