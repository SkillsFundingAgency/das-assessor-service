using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeWebsiteLinkWithErrors
        : OrganisationControllerTestBaseForModel<ChangeWebsiteViewModel>
    {
        private const string InvalidWebsiteLink = "NOTAWEBSITELINK";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false);
        }

        public override async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(ChangeWebsiteViewModel.WebsiteLink), "An error message");
            sut.ModelState[nameof(ChangeWebsiteViewModel.WebsiteLink)].AttemptedValue = InvalidWebsiteLink;

            return await sut.ChangeWebsite();
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
