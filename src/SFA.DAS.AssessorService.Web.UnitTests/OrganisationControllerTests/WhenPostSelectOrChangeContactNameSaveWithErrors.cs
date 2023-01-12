using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostSelectOrChangeContactNameSaveWithErrors
        : OrganisationControllerTestBaseForInvalidModel<SelectOrChangeContactNameViewModel>
    {
        private const string InvalidPrimaryContactEmpty = "";
        private const string InvalidPrimaryContactSame = ValidPrimaryContact;
        private const string InvalidPrimaryContactWrongOrganisation = "invalid@wrongorganisation.com";
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                contactsPrivileges: new List<ContactsPrivilege>
                {
                    // the contact has the explict 'Change Organisation Details' privilege
                    ChangeOrganisationDetailsContactsPrivilege
                });

            ContactsApiClient.Setup(c => c.GetAllContactsForOrganisation(EpaoId, null))
                .ReturnsAsync(new List<ContactResponse>());

            ContactsApiClient.Setup(c => c.GetByUsername(ValidPrimaryContact))
                .ReturnsAsync(new ContactResponse
                {
                    OrganisationId = OrganisationOneId,
                    Username = ValidPrimaryContact
                });

            ContactsApiClient.Setup(c => c.GetByUsername(InvalidPrimaryContactEmpty))
                .ReturnsAsync((ContactResponse)null);

            ContactsApiClient.Setup(c => c.GetByUsername(InvalidPrimaryContactWrongOrganisation))
                .ReturnsAsync(new ContactResponse
                {
                    OrganisationId = OrganisationTwoId,
                    Username = ValidPrimaryContact
                });
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.SelectOrChangeContactName(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = InvalidPrimaryContactEmpty,
                ActionChoice = ActionChoiceSave
            });
        }

        public override async Task<IActionResult> Act(SelectOrChangeContactNameViewModel viewModel)
        {
            return await sut.SelectOrChangeContactName(viewModel);
        }

        [TestCase(InvalidPrimaryContactEmpty)]
        [TestCase(InvalidPrimaryContactWrongOrganisation)]
        public async Task Should_return_a_model_with_invalid_primary_contact(string primaryContact)
        {
            _actionResult = await Act(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = primaryContact,
                ActionChoice = ActionChoiceSave
            });

            sut.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_same_primary_contact()
        {
            _actionResult = await Act(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = InvalidPrimaryContactSame,
                ActionChoice = ActionChoiceSave
            });

            _actionResult.Should().BeOfType<RedirectToActionResult>();
        }

        [Test]
        public async Task Should_return_a_redirecttoaction_to_organisational_details_for_same_primary_contact()
        {
            _actionResult = await Act(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = InvalidPrimaryContactSame,
                ActionChoice = ActionChoiceSave

            });

            var _redirect = _actionResult as RedirectToActionResult;
            _redirect.ActionName.Should().Be(nameof(OrganisationController.OrganisationDetails));
        }
    }
}
