using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeWebsiteLinkSaveWithoutErrors : OrganisationControllerTestBase
    {
        private const string ValidWebsiteLinkDifferent = "www.adifferentvalidcompany.com";
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidateWebsiteLink(ValidWebsiteLinkDifferent)).ReturnsAsync(true);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = ValidWebsiteLinkDifferent,
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
        public async Task Should_return_a_viewresult()
        {
            _actionResult = await Act();
            _actionResult.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task Should_return_a_model()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            result.Model.Should().BeOfType<ChangeWebsiteViewModel>();
        }

        [Test]
        public async Task Should_return_a_viewresult_called_confirm()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangeWebsiteConfirm");
        }
    }
}
