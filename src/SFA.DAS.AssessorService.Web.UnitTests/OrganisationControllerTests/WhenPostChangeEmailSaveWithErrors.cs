using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeEmailSaveWithErrors : OrganisationControllerTestBase
    {
        private const string InvalidEmailAddress = "NOTANEMAILADDRESS";
        private const string InvalidEmailSame = ValidEmailAddress;
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidateEmailAddress(ValidEmailAddress)).ReturnsAsync(true);
            ValidateApiClient.Setup(c => c.ValidateEmailAddress(InvalidEmailAddress)).ReturnsAsync(false);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangeEmail(new ChangeEmailViewModel
            {
                Email = InvalidEmailAddress,
                ActionChoice = ActionChoiceSave
            });
        }

        public async Task<IActionResult> Act(ChangeEmailViewModel viewModel)
        {
            return await sut.ChangeEmail(viewModel);
        }

        [Test]
        public async Task Should_get_an_organisation_by_epao()
        {
            _actionResult = await Act();
            OrganisationApiClient.Verify(a => a.GetEpaOrganisation(EpaoId));
        }

        [Test]
        public async Task Should_return_a_redirecttoaction()
        {
            _actionResult = await Act();
            _actionResult.Should().BeOfType<RedirectToActionResult>();
        }

        [TestCase(InvalidEmailAddress)]
        public async Task Should_return_a_model_with_invalid_email(string emailAddress)
        {
            _actionResult = await Act(new ChangeEmailViewModel
            {
                Email = emailAddress,
                ActionChoice = ActionChoiceSave
            });

            sut.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_same_email()
        {
            _actionResult = await Act(new ChangeEmailViewModel
            {
                Email = InvalidEmailSame,
                ActionChoice = ActionChoiceSave
            });

            _actionResult.Should().BeOfType<RedirectToActionResult>();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_to_organisational_details_for_same_email()
        {
            _actionResult = await Act(new ChangeEmailViewModel
            {
                Email = InvalidEmailSame,
                ActionChoice = ActionChoiceSave
            });

            var _redirect = _actionResult as RedirectToActionResult;
            _redirect.ActionName.Should().Be(nameof(OrganisationController.OrganisationDetails));
        }
    }
}
