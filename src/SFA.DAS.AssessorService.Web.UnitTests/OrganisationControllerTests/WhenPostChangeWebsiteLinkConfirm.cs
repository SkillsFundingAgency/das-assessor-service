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
    public class WhenPostChangeWebsiteLinkConfirm : OrganisationControllerTestBase
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

        public async Task<IActionResult> Act()
        {
            return await sut.ChangeWebsite(new ChangeWebsiteViewModel
            {
                WebsiteLink = ValidWebsiteLinkDifferent,
                ActionChoice = ActionChoiceConfirm
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
