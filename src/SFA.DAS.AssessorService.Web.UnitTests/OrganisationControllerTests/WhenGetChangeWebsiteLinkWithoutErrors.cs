using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeWebsiteLinkWithoutErrors
        : OrganisationControllerTestBaseForModel<ChangeWebsiteViewModel>
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                contactsPrivileges: null);
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.ChangeWebsite();
        }

        [Test]
        public async Task Should_return_a_model_with_current_website_link()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeWebsiteViewModel;

            model.WebsiteLink.Should().Be(EpaOrganisation.OrganisationData?.WebsiteLink);
        }
    }
}
