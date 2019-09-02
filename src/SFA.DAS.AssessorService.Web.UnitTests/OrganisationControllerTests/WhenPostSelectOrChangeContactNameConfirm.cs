using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostSelectOrChangeContactNameConfirm
        : OrganisationControllerTestBaseForModel<SelectOrChangeContactNameViewModel>
    {
        private Guid ValidPrimaryContactIdSameOrganisation = Guid.NewGuid();
        private const string ValidPrimaryContactSameOrganisation = "another@valid.com";
        private const string ActionChoiceConfirm = "Confirm";

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

            ContactsApiClient.Setup(c => c.GetAllContactsForOrganisation(EpaoId))
                .ReturnsAsync(new List<ContactResponse>());

            ContactsApiClient.Setup(c => c.GetByUsername(ValidPrimaryContact))
                .ReturnsAsync(new ContactResponse
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = OrganisationOneId,
                    Username = ValidPrimaryContact
                });

            ContactsApiClient.Setup(c => c.GetByUsername(ValidPrimaryContactSameOrganisation))
                .ReturnsAsync(new ContactResponse
                {
                    Id = ValidPrimaryContactIdSameOrganisation,
                    OrganisationId = OrganisationOneId,
                    Username = ValidPrimaryContactSameOrganisation
                });

            OrganisationApiClient.Setup(c => c.UpdateEpaOrganisationPrimaryContact(It.IsAny<UpdateEpaOrganisationPrimaryContactRequest>()))
                .ReturnsAsync(new List<ContactResponse>());
        }

        public override async Task<IActionResult> Act()
        {            
            return await sut.SelectOrChangeContactName(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = ValidPrimaryContactSameOrganisation,
                ActionChoice = ActionChoiceConfirm
            });
        }

        public override async Task<IActionResult> Act(SelectOrChangeContactNameViewModel viewModel)
        {
            return await sut.SelectOrChangeContactName(viewModel);
        }

        [Test]
        public async Task Should_update_primary_contact()
        {
            _actionResult = await Act();

            OrganisationApiClient.Verify(c => c.UpdateEpaOrganisationPrimaryContact(
                It.Is<UpdateEpaOrganisationPrimaryContactRequest>(p => p.OrganisationId == EpaoId && p.PrimaryContactId == ValidPrimaryContactIdSameOrganisation)), 
                Times.Once);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_updated()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("SelectOrChangeContactNameUpdated");
        }
    }
}
