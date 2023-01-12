using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangePhoneNumberConfirm
        : OrganisationControllerTestBaseForModel<ChangePhoneNumberViewModel>
    {
        private const string ValidPhoneNumberDifferent = "9876543210";
        private const string ActionChoiceConfirm = "Confirm";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                contactsPrivileges: null);

            OrganisationApiClient.Setup(c => c.UpdateEpaOrganisationPhoneNumber(It.IsAny<UpdateEpaOrganisationPhoneNumberRequest>()))
                .ReturnsAsync(new List<ContactResponse>());
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.ChangePhoneNumber(new ChangePhoneNumberViewModel
            {
                PhoneNumber = ValidPhoneNumberDifferent,
                ActionChoice = ActionChoiceConfirm
            });
        }

        public override async Task<IActionResult> Act(ChangePhoneNumberViewModel viewModel)
        {
            return await sut.ChangePhoneNumber(viewModel);
        }

        [Test]
        public async Task Should_update_phone_number()
        {
            _actionResult = await Act();

            OrganisationApiClient.Verify(c => c.UpdateEpaOrganisationPhoneNumber(
                It.Is<UpdateEpaOrganisationPhoneNumberRequest>(p => p.OrganisationId == EpaoId && p.PhoneNumber == ValidPhoneNumberDifferent)),
                Times.Once);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_updated()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangePhoneNumberUpdated");
        }
    }
}
