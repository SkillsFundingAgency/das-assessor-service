using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_CheckWithdrawalRequest_is_posted_to : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task And_Yes_Is_Selected_For_Versions_Then_Application_Is_Created()
        {
            // Arrange
            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                    });

            var applicationId = Guid.NewGuid();
            _mockApplicationApiClient
                .Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                .ReturnsAsync(applicationId);

            var model = new CheckWithdrawalRequestViewModel()
            {
                Continue = "yes"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", "1.1,1.2", null, model) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplicationController.Sequence));
            result.ControllerName.Should().Be(nameof(ApplicationController).RemoveController());
            result.RouteValues.GetValueOrDefault("Id").Should().Be(applicationId);

            _mockApplicationApiClient.Verify(m => m.UpdateStandardData(applicationId, It.IsAny<int>(), "ST0001", It.IsAny<string>(), 
                It.Is<List<string>>(x => x.Contains("1.1") && x.Contains("1.2")), StandardApplicationTypes.VersionWithdrawal));
        }

        [Test]
        public async Task And_Yes_Is_Selected_For_Standard_Then_Application_Is_Created()
        {
            // Arrange
            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
               .ReturnsAsync(new List<ApplicationResponse>());

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                    });

            var applicationId = Guid.NewGuid();
            _mockApplicationApiClient
                .Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                .ReturnsAsync(applicationId);

            var model = new CheckWithdrawalRequestViewModel()
            {
                Continue = "yes"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", null, null, model) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplicationController.Sequence));
            result.ControllerName.Should().Be(nameof(ApplicationController).RemoveController());
            result.RouteValues.GetValueOrDefault("Id").Should().Be(applicationId);

            _mockApplicationApiClient.Verify(m => m.UpdateStandardData(applicationId, It.IsAny<int>(), "ST0001", It.IsAny<string>(),
               null, StandardApplicationTypes.StandardWithdrawal));
        }

        [Test]
        public async Task And_Yes_Is_Selected_For_Standard_And_InProgress_Applications_Then_Version_Applications_Are_Deleted()
        {
            var applicationId = Guid.NewGuid();
            var existingApplicationId1 = Guid.NewGuid();
            var existingApplicationId2 = Guid.NewGuid();

            // Arrange
            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
               .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
                        Id = existingApplicationId1,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplicationType = ApplicationTypes.StandardWithdrawal,
                        StandardReference = "ST0001",
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.0" } }
                        }
                    },
                    new ApplicationResponse()
                    {
                        Id = existingApplicationId2,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplicationType = ApplicationTypes.StandardWithdrawal,
                        StandardReference = "ST0001",
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.1" } }
                        }
                    }
                });

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                    });

            
            _mockApplicationApiClient
                .Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                .ReturnsAsync(applicationId);

            var model = new CheckWithdrawalRequestViewModel()
            {
                Continue = "yes"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", null, null, model) as RedirectToActionResult;

            // Assert
            _mockApplicationApiClient.Verify(m => m.DeleteApplications(
                It.Is<DeleteApplicationsRequest>(x => x.ApplicationIds.Any(a => a == existingApplicationId1) &&
                                                        x.ApplicationIds.Any(a => a == existingApplicationId2))));
        }

        [Test]
        public async Task And_No_Is_Selected_Then_Redirected_To_WithdrawalApplications()
        {
            // Arrange
            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                   .ReturnsAsync(new List<StandardVersion>()
                   {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                   });

            var model = new CheckWithdrawalRequestViewModel()
            {
                Continue = "No"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", "1.1,1.2", null, model) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.WithdrawalApplications));
            result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
        }

        [Test]
        public async Task And_Nothing_Is_Selected_Then_Error_Is_Returned()
        {
            // Arrange
            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                   .ReturnsAsync(new List<StandardVersion>()
                   {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                   });

            var model = new CheckWithdrawalRequestViewModel()
            {
                Continue = null
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", "1.1,1.2", null, model) as RedirectToActionResult;

            // Assert
            _sut.ModelState.IsValid.Should().BeFalse();
            _sut.ModelState["Continue"].Errors.Should().Contain(x => x.ErrorMessage == "Select Yes or No");
        }
    }
}