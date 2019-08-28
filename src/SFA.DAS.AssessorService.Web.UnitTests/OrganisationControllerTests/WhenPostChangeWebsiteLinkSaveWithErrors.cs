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
    public class WhenPostChangeWebsiteLinkSaveWithErrors
        : OrganisationControllerTestBaseForInvalidModel<ChangeWebsiteViewModel>
    {
        private const string InvalidWebsiteLinkAddress = "NOTAWEBSITELINK";
        private const string InvalidWebsiteLinkSame = ValidWebsiteLink;
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidateWebsiteLink(ValidWebsiteLink)).ReturnsAsync(true);
            ValidateApiClient.Setup(c => c.ValidateWebsiteLink(InvalidWebsiteLinkAddress)).ReturnsAsync(false);
        }

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = InvalidWebsiteLinkAddress,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(ChangeWebsiteViewModel viewModel)
        {
            return await sut.ChangeWebsite(viewModel);
        }

        [TestCase(InvalidWebsiteLinkAddress)]
        public async Task Should_return_a_model_with_invalid_website_link(string websiteLink)
        {
            _actionResult = await Act(new ChangeWebsiteViewModel
            {
                WebsiteLink = websiteLink,
                ActionChoice = ActionChoiceSave
            });

            sut.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_same_website_link()
        {
            _actionResult = await Act(new ChangeWebsiteViewModel
            {
                WebsiteLink = InvalidWebsiteLinkSame,
                ActionChoice = ActionChoiceSave
            });

            _actionResult.Should().BeOfType<RedirectToActionResult>();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_to_organisational_details_for_same_website_link()
        {
            _actionResult = await Act(new ChangeWebsiteViewModel
            {
                WebsiteLink = InvalidWebsiteLinkSame,
                ActionChoice = ActionChoiceSave
            });

            var _redirect = _actionResult as RedirectToActionResult;
            _redirect.ActionName.Should().Be(nameof(OrganisationController.OrganisationDetails));
        }
    }
}
