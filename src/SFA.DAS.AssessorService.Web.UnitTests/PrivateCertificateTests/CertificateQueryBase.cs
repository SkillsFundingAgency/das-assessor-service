using System;
using System.Security.Claims;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests
{
    public class CertificateQueryBase
    {
        protected Mock<ILogger<CertificateController>> MockLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        protected Mock<ISessionService> MockSession;

        protected ICertificateApiClient MockCertificateApiClient;


        protected Certificate Certificate;
        protected CertificateData CertificateData;

        public CertificateQueryBase()
        {
            MockLogger = new Mock<ILogger<CertificateController>>();
            var mockedApiClientLogger = new Mock<ILogger<CertificateApiClient>>();

            MockHttpContextAccessor = SetupMockedHttpContextAccessor();
            MockCertificateApiClient = SetupCertificateApiClient(mockedApiClientLogger);

            MockSession = new Mock<ISessionService>();

            CertificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);
        }

        private static Mock<IHttpContextAccessor> SetupMockedHttpContextAccessor()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "jcoxhead")
            }));

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

        private CertificateApiClient SetupCertificateApiClient(Mock<ILogger<CertificateApiClient>> apiClientLoggerMock)
        {
            Certificate = SetupCertificate();

            var tokenServiceMock = new Mock<ITokenService>();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{Certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/{Certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            var apiClient = new CertificateApiClient(client, tokenServiceMock.Object, apiClientLoggerMock.Object);          

            return apiClient;
        }

        private Certificate SetupCertificate()
        {
            var certificateId = Guid.NewGuid();
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.AchievementDate = DateTime.Now)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>()
                .Build();

            certificate.Organisation = organisation;

            return certificate;
        }
    }
}