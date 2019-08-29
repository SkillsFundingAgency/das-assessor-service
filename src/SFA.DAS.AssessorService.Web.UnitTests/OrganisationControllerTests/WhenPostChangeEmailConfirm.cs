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
    public class WhenPostChangeEmailConfirm
        : OrganisationControllerTestBaseForModel<ChangeEmailViewModel>
    {
        private const string ValidEmailDifferent = "newemail@contactcompany.com";
        private const string ActionChoiceConfirm = "Confirm";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            OrganisationApiClient.Setup(c => c.UpdateEpaOrganisationEmail(It.IsAny<UpdateEpaOrganisationEmailRequest>()))
                .ReturnsAsync(new List<ContactResponse>());
        }

        public override async Task<IActionResult> Act()
        {            
            return await sut.ChangeEmail(new ChangeEmailViewModel
            {
                Email = ValidEmailDifferent,
                ActionChoice = ActionChoiceConfirm
            });
        }

        public override async Task<IActionResult> Act(ChangeEmailViewModel viewModel)
        {
            return await sut.ChangeEmail(viewModel);
        }

        [Test]
        public async Task Should_update_email_address()
        {
            _actionResult = await Act();

            OrganisationApiClient.Verify(c => c.UpdateEpaOrganisationEmail(
                It.Is<UpdateEpaOrganisationEmailRequest>(p => p.OrganisationId == EpaoId && p.Email == ValidEmailDifferent)), 
                Times.Once);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_updated()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangeEmailUpdated");
        }
    }
}
