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
    public class WhenPostChangePhoneNumberSaveWithErrors : OrganisationControllerTestBase
    {
        private const string InvalidPhoneNumber = "NOTAPHONENUMBER";
        private const string InvalidPhoneNumberSame = ValidPhoneNumber;
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidatePhoneNumber(ValidPhoneNumber)).ReturnsAsync(true);
            ValidateApiClient.Setup(c => c.ValidatePhoneNumber(InvalidPhoneNumber)).ReturnsAsync(false);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangePhoneNumber(new ChangePhoneNumberViewModel
            {
                PhoneNumber = InvalidPhoneNumber,
                ActionChoice = ActionChoiceSave
            });
        }

        public async Task<IActionResult> Act(ChangePhoneNumberViewModel viewModel)
        {
            return await sut.ChangePhoneNumber(viewModel);
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

        [TestCase(InvalidPhoneNumber)]
        public async Task Should_return_a_model_with_invalid_phone_number(string phoneNumber)
        {
            _actionResult = await Act(new ChangePhoneNumberViewModel
            {
                PhoneNumber = phoneNumber,
                ActionChoice = ActionChoiceSave
            });

            sut.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_same_phone_number()
        {
            _actionResult = await Act(new ChangePhoneNumberViewModel
            {
                PhoneNumber = InvalidPhoneNumberSame,
                ActionChoice = ActionChoiceSave
            });

            _actionResult.Should().BeOfType<RedirectToActionResult>();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_to_organisational_details_for_same_phone_number()
        {
            _actionResult = await Act(new ChangePhoneNumberViewModel
            {
                PhoneNumber = InvalidPhoneNumberSame,
                ActionChoice = ActionChoiceSave
            });

            var _redirect = _actionResult as RedirectToActionResult;
            _redirect.ActionName.Should().Be(nameof(OrganisationController.OrganisationDetails));
        }
    }
}
