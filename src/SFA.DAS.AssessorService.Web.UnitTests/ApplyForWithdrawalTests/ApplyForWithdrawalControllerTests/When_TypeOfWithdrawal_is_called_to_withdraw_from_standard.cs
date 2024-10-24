using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
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
            var result = _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.StandardWithdrawal }) as RedirectToRouteResult;

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.RouteName.Should().Be(ApplyForWithdrawalController.ChooseStandardForWithdrawalRouteGet);
            }
        }
    }
}