using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_WholeStandardOrVersion_is_posted_to : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public void And_Whole_Standard_Is_Selected_Then_Redirected_To_CheckWithdrawalRequest()
        {
            // Arrange
            var model = new WholeStandardOrVersionViewModel()
            {
                WithdrawalType = WithdrawalType.WholeStandard
            };
                
            // Act
            var result = _sut.WholeStandardOrVersion("ST0001", model) as RedirectToActionResult;

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
        }

        [Test]
        public void And_Specific_Version_Is_Selected_Then_Redirected_To_ReviewStandardVersions()
        {
            // Arrange
            var model = new WholeStandardOrVersionViewModel()
            {
                WithdrawalType = WithdrawalType.SpecificVersions
            };

            // Act
            var result = _sut.WholeStandardOrVersion("ST0001", model) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.ReviewStandardVersions));
            // result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
            // result.RouteValues.GetValueOrDefault("iFateReferenceNumber").Should().Be("ST0001");
        }

        [Test]
        public void And_No_Option_Is_Selected_Then_Error_Is_Returned()
        {
            // Arrange
            var model = new WholeStandardOrVersionViewModel()
            {
                WithdrawalType = null
            };

            // Act
            var result = _sut.WholeStandardOrVersion("ST0001", model) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // _sut.ModelState.IsValid.Should().BeFalse();
            // _sut.ModelState["WithdrawalType"].Errors.Should().Contain(x => x.ErrorMessage == "Select whole standard or version(s)");
        }
    }
}