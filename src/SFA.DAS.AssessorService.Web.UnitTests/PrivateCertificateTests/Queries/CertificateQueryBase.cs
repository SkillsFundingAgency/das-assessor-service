using System;
using System.Collections.Generic;
using System.Security.Claims;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class CertificateQueryBase
    {
        protected Mock<ILogger<CertificateController>> MockLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        protected Mock<ISessionService> MockSession;

        protected ICertificateApiClient MockCertificateApiClient;
        protected IAssessmentOrgsApiClient MockAssessmentOrgsApiClient;


        protected Certificate Certificate;
        protected CertificateData CertificateData;

        public CertificateQueryBase()
        {
            MockLogger = new Mock<ILogger<CertificateController>>();
            var mockedApiClientLogger = new Mock<ILogger<CertificateApiClient>>();

            MockHttpContextAccessor = SetupMockedHttpContextAccessor();
            MockCertificateApiClient = SetupCertificateApiClient(mockedApiClientLogger);
            MockAssessmentOrgsApiClient = SetupAssessmentOrgsApiClient(mockedApiClientLogger);

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

        private IAssessmentOrgsApiClient SetupAssessmentOrgsApiClient(Mock<ILogger<CertificateApiClient>> mockedApiClientLogger)
        {
            var standardOrganisartionSummaries = new List<StandardOrganisationSummary>
            {
                new StandardOrganisationSummary
                {
                    StandardCode = "93"
                },
                new StandardOrganisationSummary
                {
                    StandardCode = "92"
                },
                new StandardOrganisationSummary
                {
                    StandardCode = "91"
                }
            };


            var standards = new List<Standard>
            {
                new Standard
                {
                    Id = 91,
                    Level = 2,
                    Title = "Test Title 1"
                },
                new Standard
                {
                    Id = 92,
                    Level = 3,
                    Title = "Test Title 2"
                },
                new Standard
                {
                    Id = 93,
                    Level = 5,
                    Title = "Test Title 3"
                },
                new Standard
                {
                    Id = 94,
                    Level = 2,
                    Title = "Test Title 4"
                },
                new Standard
                {
                    Id = 95,
                    Level = 2,
                    Title = "Test Title 5"
                },
            };

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("https://findapprenticeshiptraining-api.sfa.bis.gov.uk/");

            mockHttp.When($"/assessment-organisations/EPA00001/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standardOrganisartionSummaries));

            mockHttp.When($"https://findapprenticeshiptraining-api.sfa.bis.gov.uk/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standards));

            var apiClient = new AssessmentOrgsApiClient(client);

            return apiClient;
        }

        private Certificate SetupCertificate()
        {
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

        protected void SetupSession()
        {
            var certificateSession = Builder<CertificateSession>
                .CreateNew()
                .With(q => q.CertificateId = Certificate.Id)
                .Build();

            var serialisedCertificateSession
                = JsonConvert.SerializeObject(certificateSession);

            MockSession.Setup(q => q.Get("CertificateSession"))
                .Returns(serialisedCertificateSession);

            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId"))
                .Returns("EPA00001");
        }

        protected void AddRedirectCheck()
        {
            var queryString = new Dictionary<string, StringValues> { { "redirecttocheck", "true" } };

            MockHttpContextAccessor.Object.HttpContext.Request.Query =
                new QueryCollection(queryString);
        }
    }
}