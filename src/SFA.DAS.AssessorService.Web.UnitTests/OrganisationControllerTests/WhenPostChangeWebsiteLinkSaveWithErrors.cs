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
    public class WhenPostChangeWebsiteLinkSaveWithErrors : OrganisationControllerTestBase
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

        public async Task<IActionResult> Act()
        {
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = InvalidWebsiteLinkAddress,
                ActionChoice = ActionChoiceSave
            });
        }

        public async Task<IActionResult> Act(ChangeWebsiteViewModel viewModel)
        {
            return await sut.ChangeWebsite(viewModel);
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
