using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeWebsiteLinkWithErrors : OrganisationControllerTestBase
    {
        private const string InvalidWebsiteLink = "NOTAWEBSITELINK";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false);
        }

        public async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(ChangeWebsiteViewModel.WebsiteLink), "An error message");
            sut.ModelState[nameof(ChangeWebsiteViewModel.WebsiteLink)].AttemptedValue = InvalidWebsiteLink;

            return await sut.ChangeWebsite();
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
        public async Task Should_return_a_model_with_invalid_email()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeWebsiteViewModel;

            // the website link is replaced with the invalid one to be replayed in the view
            model.WebsiteLink.Should().Be(InvalidWebsiteLink);
        }
    }
}
