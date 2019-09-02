using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeWebsiteLinkSaveWithoutErrors
        : OrganisationControllerTestBaseForModel<ChangeWebsiteViewModel>
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

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = ValidWebsiteLinkDifferent,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(ChangeWebsiteViewModel viewModel)
        {
            return await sut.ChangeWebsite(viewModel);
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
