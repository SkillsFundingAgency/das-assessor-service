using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Tests.MockedObjects;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class CertificateQueryBase
    {
        protected Mock<ILogger<CertificateAmendController>> MockLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        protected ApiClient MockApiClient;
        protected IAssessmentOrgsApiClient MockAssessmentOrgsApiClient;        

        protected Certificate Certificate;
        protected CertificateData CertificateData;
        protected Mock<IStandardServiceClient> MockStandardServiceClient;

        public CertificateQueryBase()
        {
            Certificate = SetupCertificate();

            MockLogger = new Mock<ILogger<CertificateAmendController>>();
            var mockedApiClientLogger = new Mock<ILogger<ApiClient>>();            

            MockHttpContextAccessor = MockedHttpContextAccessor.Setup();
            MockApiClient = MockedApiClient.Setup(Certificate, mockedApiClientLogger);
            MockStandardServiceClient = new Mock<IStandardServiceClient>();

            var standards = new List<StandardSummary>
            {
                new StandardSummary
                {
                    Id = "91",
                    Level = 2,
                    Title = "Test Title 1"
                },
                new StandardSummary
                {
                    Id = "92",
                    Level = 3,
                    Title = "Test Title 2"
                },
                new StandardSummary
                {
                    Id = "93",
                    Level = 5,
                    Title = "Test Title 3"
                },
                new StandardSummary
                {
                    Id = "94",
                    Level = 2,
                    Title = "Test Title 4"
                },
                new StandardSummary
                {
                    Id = "95",
                    Level = 2,
                    Title = "Test Title 5"
                },
            };

            MockStandardServiceClient.Setup(s => s.GetAllStandardSummaries()).Returns(Task.FromResult(standards.AsEnumerable()));



            MockAssessmentOrgsApiClient = MockedAssessmentOrgsApiClient.Setup(mockedApiClientLogger);

            CertificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);
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
       
        protected void AddRedirectCheck()
        {
            var queryString = new Dictionary<string, StringValues> { { "redirecttocheck", "true" } };

            MockHttpContextAccessor.Object.HttpContext.Request.Query =
                new QueryCollection(queryString);
        }
    }
}