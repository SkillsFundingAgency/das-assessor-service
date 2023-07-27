using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_ReviewStandardVersions_is_called : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task Then_StandardVersions_Are_Returned()
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
                            Apply = new ApplyTypes.Apply() { StandardReference = "ST0001", Versions = new List<string> { "1.0", "1.1" } }
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
                
            // Act
            var result = await _sut.ReviewStandardVersions("ST0001") as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // var model = result.Model as ReviewStandardVersionsViewModel;
            // model.Versions.Should().HaveCount(3);
            // model.Versions[0].Version.Should().Be("1.2");
            // model.Versions[0].AbleToWithdraw.Should().BeTrue();
            // model.Versions[1].AbleToWithdraw.Should().BeFalse();
            // model.Versions[2].AbleToWithdraw.Should().BeFalse();
        }
    }
}