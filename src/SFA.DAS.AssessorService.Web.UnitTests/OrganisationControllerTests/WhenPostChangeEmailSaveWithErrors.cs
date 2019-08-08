using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeEmailSaveWithErrors
        : OrganisationControllerTestBaseForInvalidModel<ChangeEmailViewModel>
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

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeEmail(new ChangeEmailViewModel
            {
                Email = InvalidEmailAddress,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(ChangeEmailViewModel viewModel)
        {
            return await sut.ChangeEmail(viewModel);
        }

        [TestCase(InvalidEmailAddress)]
        [TestCase(InvalidEmailSame)]
        public async Task Should_return_a_model_with_invalid_email(string emailAddress)
        {
            _actionResult = await Act(new ChangeEmailViewModel
            {
                Email = emailAddress,
                ActionChoice = ActionChoiceSave
            });

            sut.ModelState.IsValid.Should().BeFalse();
        }
    }
}
