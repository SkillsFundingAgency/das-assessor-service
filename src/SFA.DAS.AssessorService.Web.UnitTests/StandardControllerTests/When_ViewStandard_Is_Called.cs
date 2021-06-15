using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ViewStandard_Is_Called
    {
        private StandardController _sut;
        private Mock<IApplicationApiClient> _mockApiClient;
        private Mock<IOrganisationsApiClient> _mockOrgApiClient;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IContactsApiClient> _mockContactsApiClient;
        private Mock<IStandardVersionClient> _mockStandardVersionApiClient;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IApplicationService> _mockApplicationService;
        private Mock<IWebConfiguration> _mockConfig;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApplicationApiClient>();
            _mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockStandardVersionApiClient = new Mock<IStandardVersionClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockApplicationService = new Mock<IApplicationService>();
            _mockConfig = new Mock<IWebConfiguration>();

            _mockHttpContextAccessor
                .Setup(r => r.HttpContext)
                .Returns(SetupHttpContextSubAuthorityClaim());

            _mockApiClient
             .Setup(r => r.GetApplication(It.IsAny<Guid>()))
             .ReturnsAsync(new ApplicationResponse()
             {
                 ApplicationStatus = ApplicationStatus.InProgress,
                 ApplyData = new ApplyData()
                 {
                     Sequences = new List<ApplySequence>()
                     {
                         new ApplySequence()
                         {
                             IsActive = true,
                             SequenceNo = ApplyConst.STANDARD_SEQUENCE_NO,
                             Status = ApplicationSequenceStatus.Draft
                         }
                    }
                 }
             });


            _mockContactsApiClient.Setup(r => r.GetContactBySignInId(It.IsAny<String>()))
             .ReturnsAsync(new ContactResponse());

            _mockOrgApiClient
             .Setup(r => r.GetOrganisationByUserId(It.IsAny<Guid>()))
             .ReturnsAsync(new OrganisationResponse());

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
               _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockApplicationService.Object,
               _mockHttpContextAccessor.Object, _mockConfig.Object);
        }

        [Test]
        public async Task Then_Use_exisiting_Application_if_exisits()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApiClient
            .Setup(r => r.GetStandardApplications(It.IsAny<Guid>()))
            .ReturnsAsync(new List<ApplicationResponse>()
            {
                new ApplicationResponse() { Id = applicationId, StandardCode = null },
                new ApplicationResponse() { Id = Guid.NewGuid(), StandardCode = 123 }
            });

            // Act
            var results = (await _sut.ViewStandard("ST0001")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("ConfirmStandard", results.ActionName);
            Assert.AreEqual(applicationId, results.RouteValues["Id"]);
        }

        [Test]
        public async Task Then_Use_new_Application_if_none_exisits()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApiClient
            .Setup(r => r.GetStandardApplications(It.IsAny<Guid>()))
            .ReturnsAsync(new List<ApplicationResponse>()
            {
                new ApplicationResponse() { Id = Guid.NewGuid(), StandardCode = 456 },
                new ApplicationResponse() { Id = Guid.NewGuid(), StandardCode = 123 }
            });

            _mockApplicationService.Setup(r => r.BuildInitialRequest(It.IsAny<ContactResponse>(), It.IsAny<OrganisationResponse>(), It.IsAny<string>()))
                        .ReturnsAsync(new CreateApplicationRequest());
            _mockApiClient.Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                        .ReturnsAsync(applicationId);

            // Act
            var results = (await _sut.ViewStandard("ST0001")) as RedirectToActionResult;

            // Assert
            Assert.AreEqual("ConfirmStandard", results.ActionName);
            Assert.AreEqual(applicationId, results.RouteValues["Id"]);
        }

        private HttpContext SetupHttpContextSubAuthorityClaim()
        {
            var fakeClaims = new List<Claim>()
            {
                new Claim("sub", "")
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
