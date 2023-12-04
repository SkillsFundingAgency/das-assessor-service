using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_ChooseStandardVersionForWithdrawal_is_posted_to : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task Then_Redirected_To_CheckWithdrawalRequest()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
                        Id = applicationId,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.0" } }
                        }
                    }
                });

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" },
                        new StandardVersion() { Version = "1.3" }
                    });

            var model = new ChooseStandardVersionForWithdrawalViewModel()
            {
                SelectedVersions = new List<string>() {"1.1", "1.2"}
            };

            // Act
            var result = await _sut.ChooseStandardVersionForWithdrawal("ST0001", model) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.CheckWithdrawalRequest));
            // result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
            // result.RouteValues.GetValueOrDefault("iFateReferenceNumber").Should().Be("ST0001");
            // result.RouteValues.GetValueOrDefault("versionsToWithdrawal").Should().Be("1.1,1.2");
        }

        [Test]
        public async Task And_No_Versions_Are_Selected_Then_Error_Is_Returned()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
                        Id = applicationId,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.0" } }
                        }
                    }
                });

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" },
                        new StandardVersion() { Version = "1.3" }
                    });

            var model = new ChooseStandardVersionForWithdrawalViewModel()
            {
                SelectedVersions = new List<string>()
            };

            // Act
            var result = await _sut.ChooseStandardVersionForWithdrawal("ST0001", model) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // _sut.ModelState.IsValid.Should().BeFalse();
            // _sut.ModelState["SelectedVersions"].Errors.Should().Contain(x => x.ErrorMessage == "Select at least one version");
        }

        [Test]
        public async Task And_All_Versions_Are_Selected_Then_Server_Withdrawal_Is_Used()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
                        Id = applicationId,
                        ApplicationStatus = ApplicationStatus.InProgress,
                        ApplicationType = ApplicationTypes.StandardWithdrawal,
                        StandardReference = "ST0001",
                        ApplyData = new ApplyData()
                        {
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.0" } }
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

            var model = new ChooseStandardVersionForWithdrawalViewModel()
            {
                SelectedVersions = new List<string>() { "1.1", "1.2" }
            };

            // Act
            var result = await _sut.ChooseStandardVersionForWithdrawal("ST0001", model) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.CheckWithdrawalRequest));
            // result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
            // result.RouteValues.GetValueOrDefault("iFateReferenceNumber").Should().Be("ST0001");
            // result.RouteValues.GetValueOrDefault("versionsToWithdrawal").Should().BeNull();
        }
    }
}