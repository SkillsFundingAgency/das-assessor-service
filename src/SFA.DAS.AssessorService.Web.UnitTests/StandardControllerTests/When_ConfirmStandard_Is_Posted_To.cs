using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ConfirmStandard_Is_Posted_To
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
             .Setup(r => r.GetEpaOrganisationById(It.IsAny<String>()))
             .ReturnsAsync(new EpaOrganisation()
             {
                 OrganisationId = "12345"
             });

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
               _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockApplicationService.Object,
               _mockHttpContextAccessor.Object, _mockConfig.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Test]
        public async Task Then_UpdateStandardData_Is_Called()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetStandardVersionsByOrganisationIdAndStandardReference(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
                SelectedVersions = new List<string>() { "1.1" }
            };
            await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null);

            // Assert
            _mockApiClient.Verify(m => m.UpdateStandardData(It.IsAny<Guid>(), 1, "ST0001", "Title 1",
                It.Is<List<string>>(x => x.Count == 1 && x[0] == "1.1"), null));
        }

        [Test]
        public async Task Then_Error_If_Confirmed_Is_False()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetStandardVersionsByOrganisationIdAndStandardReference(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = false,
                SelectedVersions = new List<string>() { "1.1" }
            };
            var result = (await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null)) as ViewResult;

            // Assert
            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", result.ViewName);
            Assert.IsTrue(_sut.ModelState["IsConfirmed"].Errors.Any(x => x.ErrorMessage == "Please tick to confirm"));
        }

        [Test]
        public async Task Then_Error_If_No_Versions_Selected()
        {
            // Arrange
            _mockOrgApiClient
              .Setup(r => r.GetStandardVersionsByOrganisationIdAndStandardReference(It.IsAny<string>(), "ST0001"))
              .ReturnsAsync(new List<AppliedStandardVersion> {
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.0M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
                   new AppliedStandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = 1.1M, LarsCode = 1, EPAChanged = false, ApprovedStatus = ApprovedStatus.NotYetApplied},
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
                SelectedVersions = new List<string>() { }
            };
            var result = (await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", null)) as ViewResult;

            // Assert
            Assert.AreEqual("~/Views/Application/Standard/ConfirmStandard.cshtml", result.ViewName);
            Assert.IsTrue(_sut.ModelState["SelectedVersions"].Errors.Any(x => x.ErrorMessage == "You must select at least one version"));
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
