using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests
{
    [TestFixture]
    public class When_TypeOfWithdrawal_is_called_to_withdraw_from_register : ApplyForWithdrawalControllerTestsBase
    {
        [Test]
        public async Task Then_Redirect_To_CheckWithdrawalRequest()
        {
            // Act
            var result = _sut.TypeOfWithdrawal(new TypeOfWithdrawalViewModel { TypeOfWithdrawal = ApplicationTypes.OrganisationWithdrawal }) as RedirectToActionResult;

            // Assert
            result.ActionName.Should().Be(nameof(ApplyForWithdrawalController.CheckWithdrawalRequest));
            result.ControllerName.Should().Be(nameof(ApplyForWithdrawalController).RemoveController());
        }
    }
}