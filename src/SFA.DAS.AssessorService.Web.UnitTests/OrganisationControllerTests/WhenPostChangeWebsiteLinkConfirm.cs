using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangeWebsiteLinkConfirm
        : OrganisationControllerTestBaseForModel<ChangeWebsiteViewModel>
    {
        private const string ValidWebsiteLinkDifferent = "www.adifferntcompany.com";
        private const string ActionChoiceConfirm = "Confirm";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            OrganisationApiClient.Setup(c => c.UpdateEpaOrganisationWebsiteLink(It.IsAny<UpdateEpaOrganisationWebsiteLinkRequest>()))
                .ReturnsAsync(new List<ContactResponse>());
        }

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = ValidWebsiteLinkDifferent,
                ActionChoice = ActionChoiceConfirm
            });
        }

        public override async Task<IActionResult> Act(ChangeWebsiteViewModel viewModel)
        {
            return await sut.ChangeWebsite(viewModel);
        }

        [Test]
        public async Task Should_update_website_link()
        {
            _actionResult = await Act();

            OrganisationApiClient.Verify(c => c.UpdateEpaOrganisationWebsiteLink(
                It.Is<UpdateEpaOrganisationWebsiteLinkRequest>(p => p.OrganisationId == EpaoId && p.WebsiteLink == ValidWebsiteLinkDifferent)), 
                Times.Once);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_updated()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangeWebsiteUpdated");
        }
    }
}
