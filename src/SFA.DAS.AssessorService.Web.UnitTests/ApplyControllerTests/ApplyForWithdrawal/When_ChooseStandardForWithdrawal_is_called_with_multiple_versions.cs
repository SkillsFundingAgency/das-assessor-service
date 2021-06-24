using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyControllerTests.ApplyForWithdrawal
{
    [TestFixture]
    public class When_ChooseStandardForWithdrawal_is_called_with_multiple_versions
    {
        private ApplyForWithdrawalController _sut;
        private Mock<IApplicationService> _mockApplicationService;
        private Mock<IOrganisationsApiClient> _mockOrganisationsApiClient;
        private Mock<IApplicationApiClient> _mockApplicationsApiClient;
        private Mock<IContactsApiClient> _mockContactsApiClient;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IStandardsApiClient> _mockStandardsApiClient;
        private Mock<IStandardVersionClient> _mockStandardVersionApiClient;
        private Mock<IWebConfiguration> _mockWebConfiguration;

        [SetUp]
        public void Arrange()
        {
            _mockApplicationService = new Mock<IApplicationService>();
            _mockOrganisationsApiClient = new Mock<IOrganisationsApiClient>();
            _mockApplicationsApiClient = new Mock<IApplicationApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockStandardsApiClient = new Mock<IStandardsApiClient>();
            _mockStandardVersionApiClient = new Mock<IStandardVersionClient>();
            _mockWebConfiguration = new Mock<IWebConfiguration>();

            _mockHttpContextAccessor
                .Setup(r => r.HttpContext)
                .Returns(SetupHttpContextSubAuthorityClaim());

            _mockContactsApiClient
                .Setup(r => r.GetContactBySignInId(It.IsAny<string>()))
                .ReturnsAsync(new ContactResponse { });

            _mockOrganisationsApiClient
                .Setup(r => r.GetOrganisationByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(new OrganisationResponse { });

            _mockApplicationService
                .Setup(r => r.BuildOrganisationWithdrawalRequest(It.IsAny<ContactResponse>(), It.IsAny<OrganisationResponse>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateApplicationRequest { });

            _mockApplicationsApiClient
                .Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                .ReturnsAsync(Guid.NewGuid());

            _mockStandardVersionApiClient.Setup(r => r.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0300"))
              .ReturnsAsync(new List<StandardVersion>()
              {
                    new StandardVersion(),
                    new StandardVersion(),
              });

            _sut = new ApplyForWithdrawalController(_mockApplicationService.Object, _mockOrganisationsApiClient.Object, _mockApplicationsApiClient.Object,
                _mockContactsApiClient.Object, _mockHttpContextAccessor.Object, _mockStandardsApiClient.Object, _mockStandardVersionApiClient.Object, _mockWebConfiguration.Object);
        }
        
        [Test]
        public async Task Then_Redirected_To_ChooseStandardVersionForWithdrawal()
        {
            // Arrange
            _mockApplicationsApiClient
                .Setup(r => r.GetStandards())
                .ReturnsAsync(new List<StandardCollation> { new StandardCollation { Id = 10, StandardId = 287, ReferenceNumber = "ST0300", Title = "A standard title" } });

            // Act
            var result = await _sut.ChooseStandardForWithdrawal(new ChooseStandardForWithdrawalViewModel { SelectedStandardForWithdrawal = 287 }) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.ChooseStandardVersionForWithdrawal));
            result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());

            result.RouteValues.GetValueOrDefault("iFateReferenceNumber").Should().Be("ST0300");
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