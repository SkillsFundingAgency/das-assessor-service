using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeEmailSaveWithoutErrors
        : OrganisationControllerTestBaseForModel<ChangeEmailViewModel>
    {
        private const string ValidEmailDifferent = "differentcontact@validcompany.com";
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidateEmailAddress(ValidEmailDifferent)).ReturnsAsync(true);
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.ChangeEmail(new ChangeEmailViewModel
            {
                Email = ValidEmailDifferent,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(ChangeEmailViewModel viewModel)
        {
            return await sut.ChangeEmail(viewModel);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_confirm()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangeEmailConfirm");
        }
    }
}
