using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeEmailWithErrors
        : OrganisationControllerTestBaseForModel<ChangeEmailViewModel>
    {
        private const string InvalidEmail = "NOTANEMAILADDRESS";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false);
        }

        public override async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(ChangeEmailViewModel.Email), "An error message");
            sut.ModelState[nameof(ChangeEmailViewModel.Email)].AttemptedValue = InvalidEmail;

            return await sut.ChangeEmail();
        }

        [Test]
        public async Task Should_return_a_model_with_invalid_email()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeEmailViewModel;

            // the email is replaced with the invalid one to be replayed in the view
            model.Email.Should().Be(InvalidEmail);
        }
    }
}
