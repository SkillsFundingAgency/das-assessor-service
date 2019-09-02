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
    public class WhenPostChangePhoneNumberSaveWithErrors
        : OrganisationControllerTestBaseForInvalidModel<ChangePhoneNumberViewModel>
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

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangePhoneNumber(new ChangePhoneNumberViewModel
            {
                PhoneNumber = InvalidPhoneNumber,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(ChangePhoneNumberViewModel viewModel)
        {
            return await sut.ChangePhoneNumber(viewModel);
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
