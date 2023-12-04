using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_TypeOfWithdrawal_is_called_to_withdraw_from_register : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public void Then_Redirect_To_CheckWithdrawalRequest()
        {
            // Act
            var result = _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.OrganisationWithdrawal }) as RedirectToRouteResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.RouteName, Is.EqualTo(ApplyForWithdrawalController.CheckWithdrawalRequestRouteGet));
                Assert.That(result.RouteValues, Contains.Item(new KeyValuePair<string, object>("backRouteName", ApplyForWithdrawalController.TypeofWithdrawalRouteGet)));
            });
        }
    }
}