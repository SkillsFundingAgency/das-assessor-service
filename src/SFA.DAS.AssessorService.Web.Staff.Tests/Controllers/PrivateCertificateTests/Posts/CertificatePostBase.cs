using System;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Tests.MockedObjects;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class CertificatePostBase
    {
        protected Mock<ILogger<CertificateAmendController>> MockLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        protected ApiClient MockApiClient;
        protected IAssessmentOrgsApiClient MockAssessmentOrgsApiClient;

        protected Mock<ISessionService> MockSession;

        protected Certificate Certificate;
        protected CertificateData CertificateData;

        public CertificatePostBase()
        {
            Certificate = SetupCertificate();

            MockLogger = new Mock<ILogger<CertificateAmendController>>();
            var mockedApiClientLogger = new Mock<ILogger<ApiClient>>();

            MockSession = new Mock<ISessionService>();

            MockHttpContextAccessor = MockedHttpContextAccessor.Setup();
            MockApiClient = MockedApiClient.Setup(Certificate, mockedApiClientLogger);

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
    }
}