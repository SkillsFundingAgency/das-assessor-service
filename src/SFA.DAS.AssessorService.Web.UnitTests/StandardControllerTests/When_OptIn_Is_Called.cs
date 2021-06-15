using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_OptIn_Is_Called
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

            _mockOrgApiClient
            .Setup(r => r.GetOrganisationStandardsByOrganisation(It.IsAny<String>()))
            .ReturnsAsync(new List<OrganisationStandardSummary>());

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
               _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockApplicationService.Object,
               _mockHttpContextAccessor.Object, _mockConfig.Object);
        }

        [Test]
        public async Task Then_All_Versions_For_Standard_Are_Returned()
        {
            // Arrange
            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ST0001"))
               .ReturnsAsync(new List<StandardVersion> {
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1, EPAChanged = false},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1, EPAChanged = false},
               });                                                                                     

            // Act
            var results = (await _sut.OptIn(Guid.NewGuid(), "ST0001", 1.2M)) as ViewResult;

            // Assert
            var vm = results.Model as StandardOptInViewModel;
            Assert.AreEqual("ST0001", vm.StandardReference);
            Assert.AreEqual("1.2", vm.Version);
            Assert.AreEqual("~/Views/Application/Standard/OptIn.cshtml", results.ViewName);
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
