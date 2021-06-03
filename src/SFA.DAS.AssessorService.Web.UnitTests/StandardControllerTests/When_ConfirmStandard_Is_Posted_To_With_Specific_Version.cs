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
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_ConfirmStandard_Is_Posted_To_With_Specific_Version
    {
        private StandardController _sut;
        private Mock<IApplicationApiClient> _mockApiClient;
        private Mock<IOrganisationsApiClient> _mockOrgApiClient;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IContactsApiClient> _mockContactsApiClient;
        private Mock<IStandardVersionClient> _mockStandardVersionApiClient;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApplicationApiClient>();
            _mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockStandardVersionApiClient = new Mock<IStandardVersionClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

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
             .Setup(r => r.GetEpaOrganisation(It.IsAny<String>()))
             .ReturnsAsync(new EpaOrganisation()
             {
                 OrganisationId = "12345"
             });

            _mockOrgApiClient
            .Setup(r => r.GetOrganisationStandardsByOrganisation(It.IsAny<String>()))
            .ReturnsAsync(new List<OrganisationStandardSummary>());

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
               _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockHttpContextAccessor.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Test]
        public async Task Then_UpdateStandardData_Is_Called()
        {
            // Arrange
            _mockStandardVersionApiClient
               .Setup(r => r.GetStandardVersionsByIFateReferenceNumber("ST0001"))
               .ReturnsAsync(new List<StandardVersion> { 
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0", LarsCode = 1},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1", LarsCode = 1},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.2", LarsCode = 1},
               });

            _mockOrgApiClient
              .Setup(r => r.GetOrganisationStandardByOrganisationAndReference("12345", "ST0001"))
              .ReturnsAsync(new OrganisationStandard()
              {
                  Versions = new List<OrganisationStandardVersion>()
                  {
                       new OrganisationStandardVersion() { Status = "Live", Version = "1.1"}
                  }
              });

            // Act
            var model = new StandardVersionViewModel()
            {
                IsConfirmed = true,
            };
            await _sut.ConfirmStandard(model, Guid.NewGuid(), "ST0001", "1.2");

            // Assert
            _mockApiClient.Verify(m => m.UpdateStandardData(It.IsAny<Guid>(), 1, "ST0001", "Title 1",
                It.Is<List<string>>(x => x.Count == 1 && x[0] == "1.2"), ApplicationTypes.Version));

            _mockQnaApiClient.Verify(m => m.UpdateApplicationData(It.IsAny<Guid>(), It.Is<ApplicationData>(x => x.ApplicationType == ApplicationTypes.Version)));
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
