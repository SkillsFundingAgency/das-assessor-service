using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
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
                IfateReferenceNumber = "ST0001",
                Continue = "yes"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest(model) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.RouteName.Should().Be(ApplicationController.SequenceRouteGet);
                result.RouteValues.GetValueOrDefault("Id").Should().Be(applicationId);

                _mockApplicationApiClient.Verify(m => m.UpdateStandardData(applicationId, It.IsAny<int>(), "ST0001", It.IsAny<string>(),
                   null, StandardApplicationTypes.StandardWithdrawal));
            }
        }

        [Test]
        public async Task And_Yes_Is_Selected_For_Organisation_Withdrawal_Then_Application_Is_Created()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            _mockApplicationApiClient
                .Setup(r => r.CreateApplication(It.IsAny<CreateApplicationRequest>()))
                .ReturnsAsync(applicationId);

            var model = new CheckWithdrawalRequestViewModel()
            {
                IfateReferenceNumber = null,
                Continue = "yes"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest(model) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.RouteName.Should().Be(ApplicationController.SequenceRouteGet);
                result.RouteValues.GetValueOrDefault("Id").Should().Be(applicationId);
            }
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
                IfateReferenceNumber = "ST0001",
                Continue = "No"
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest(model) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.RouteName.Should().Be(ApplyForWithdrawalController.WithdrawalApplicationsRouteGet);
            }
        }

        [Test]
        public async Task And_Model_IsInvalid_Then_RedirectToCheckWithdrawalRequest()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(CheckWithdrawalRequestViewModel.Continue), "Error");
            _sut.ViewData.ModelState.Merge(modelState);

            _mockStandardVersionApiClient.Setup(m => m.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), "ST0001"))
                   .ReturnsAsync(new List<StandardVersion>()
                   {
                        new StandardVersion() { Version = "1.0" },
                        new StandardVersion() { Version = "1.1" },
                        new StandardVersion() { Version = "1.2" }
                   });

            var model = new CheckWithdrawalRequestViewModel()
            {
                IfateReferenceNumber = "ST0001",
                Continue = null
            };

            // Act
            var result = await _sut.CheckWithdrawalRequest(model) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.RouteName.Should().Be(ApplyForWithdrawalController.CheckWithdrawalRequestRouteGet);
                result.RouteValues.Should().Contain(new KeyValuePair<string, object>("ifateReferenceNumber", model.IfateReferenceNumber));
            }
        }
    }
}