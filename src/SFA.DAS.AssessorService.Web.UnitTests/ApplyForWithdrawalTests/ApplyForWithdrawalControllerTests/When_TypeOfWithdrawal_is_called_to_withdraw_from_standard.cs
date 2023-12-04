using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_TypeOfWithdrawal_is_called_to_withdraw_from_standard : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public void Then_Redirect_To_ChooseStandardForWithdrawal()
        {
            // Act
            var result = _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.StandardWithdrawal }) as RedirectToActionResult;

            // Temporary asserts until withdrawal functionality is restored
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo(nameof(DashboardController.Index)));
                Assert.That(result.ControllerName, Is.EqualTo(nameof(DashboardController).Replace("Controller", "")));
            });

            // Assert
            // result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.ChooseStandardForWithdrawal));
            // result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
        }
    }
}