using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    public class StandardControllerTestBase
    {
        protected StandardController _sut;
        protected Mock<IApplicationApiClient> _mockApiClient;
        protected Mock<IOrganisationsApiClient> _mockOrgApiClient;
        protected Mock<IQnaApiClient> _mockQnaApiClient;
        protected Mock<IContactsApiClient> _mockContactsApiClient;
        protected Mock<IStandardVersionClient> _mockStandardVersionApiClient;
        protected Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        protected Mock<IWebConfiguration> _mockConfig;

        protected Guid SignInId = Guid.NewGuid();
        protected Guid UserId = Guid.NewGuid();
        protected string EpaOrgId = "EPA0001";

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApplicationApiClient>();
            _mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockStandardVersionApiClient = new Mock<IStandardVersionClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockConfig = new Mock<IWebConfiguration>();

            _mockHttpContextAccessor
                .Setup(r => r.HttpContext)
                .Returns(SetupHttpContextSubAuthorityClaim(SignInId, EpaOrgId));

            _mockApiClient
                .Setup(r => r.GetApplication(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationResponse()
                {
                    ApplicationStatus = ApplicationStatus.InProgress,
                    ApplyData = new Domain.Entities.ApplyData()
                    {
                        Sequences = new List<Domain.Entities.ApplySequence>()
                        {
                            new Domain.Entities.ApplySequence()
                            {
                                IsActive = true,
                                SequenceNo = ApplyConst.STANDARD_SEQUENCE_NO,
                                Status = ApplicationSequenceStatus.Draft
                            }
                        }
                    }
                });

            _mockApiClient
                .Setup(r => r.GetStandardApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>());

            _mockQnaApiClient
                .Setup(r => r.GetApplicationData(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationData()
                {
                    OrganisationReferenceId = "12345"
                });

            _mockOrgApiClient
                .Setup(r => r.GetEpaOrganisationById(It.IsAny<String>()))
                .ReturnsAsync(new EpaOrganisation()
                {
                    OrganisationId = "12345"
                });

            _mockOrgApiClient
                .Setup(r => r.GetEpaOrganisation(It.IsAny<String>()))
                .ReturnsAsync(new EpaOrganisation()
                {
                    OrganisationId = "12345"
                });

            _mockContactsApiClient
                .Setup(r => r.GetContactBySignInId(SignInId.ToString()))
                .ReturnsAsync(new ContactResponse { Id = UserId, SignInId = SignInId });

            _mockOrgApiClient
                .Setup(r => r.GetOrganisationStandardsByOrganisation(It.IsAny<String>()))
                .ReturnsAsync(new List<OrganisationStandardSummary>());

            _mockContactsApiClient
                .Setup(r => r.GetContactBySignInId(SignInId.ToString()))
                .ReturnsAsync(new ContactResponse { Id = UserId, SignInId = SignInId });

            _mockConfig
                .Setup(r => r.FeedbackUrl)
                .Returns("http://feedback-url.com");

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
                _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockHttpContextAccessor.Object, _mockConfig.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }


        private static HttpContext SetupHttpContextSubAuthorityClaim(Guid signInId, string epaOrgId)
        {
            var fakeClaims = new List<Claim>()
            {
                new Claim("sub", signInId.ToString()),
                new Claim("http://schemas.portal.com/epaoid", epaOrgId)
            };

            var fakeIdentity = new ClaimsIdentity(fakeClaims, "TestAuthType");
            var fakeClaimsPrincipal = new ClaimsPrincipal(fakeIdentity);

            return new DefaultHttpContext
            {
                User = fakeClaimsPrincipal
            };
        }
    }
}
