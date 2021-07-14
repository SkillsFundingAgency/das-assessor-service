using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_CheckWithdrawalRequest_is_called : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task And_Versions_Are_Selected_Then_InProgressVersionWithdrawals_is_False()
        {
            // Arrange
            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
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
                        new StandardVersion() { Version = "1.2" }
                    });
                
            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", "1.1", null) as ViewResult;

            // Assert
            var model = result.Model as CheckWithdrawalRequestViewModel;
            model.InProgressVersionWithdrawals.Should().BeFalse();
        }

        [Test]
        public async Task And_Standard_Is_Selected_And_There_Are_InProgress_Applications_Then_InProgressVersionWithdrawals_is_True()
        {
            // Arrange
            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                    new ApplicationResponse()
                    {
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

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", null, null) as ViewResult;

            // Assert
            var model = result.Model as CheckWithdrawalRequestViewModel;
            model.InProgressVersionWithdrawals.Should().BeTrue();
        }

        [Test]
        public async Task And_Standard_Is_Selected_And_There_Are_No_InProgress_Applications_Then_InProgressVersionWithdrawals_is_True()
        {
            // Arrange
            _mockApplicationApiClient.Setup(m => m.GetWithdrawalApplications(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationResponse>()
                {
                });

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                    .ReturnsAsync(new List<StandardVersion>()
                    {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                    });

            // Act
            var result = await _sut.CheckWithdrawalRequest("ST0001", null, null) as ViewResult;

            // Assert
            var model = result.Model as CheckWithdrawalRequestViewModel;
            model.InProgressVersionWithdrawals.Should().BeFalse();
        }
    }
}