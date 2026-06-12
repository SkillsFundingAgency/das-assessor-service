using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostSelectOrChangeContactNameSaveWithoutErrors : OrganisationControllerTestBase
    {
        private const string ValidPrimaryContactSameOrganisation = "another@valid.com";
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

            ContactsApiClient.Setup(c => c.GetByUsername(ValidPrimaryContactSameOrganisation))
                .ReturnsAsync(new ContactResponse
                {
                    OrganisationId = OrganisationOneId,
                    Username = ValidPrimaryContactSameOrganisation
                });
        }

        public async Task<IActionResult> Act()
        {
            return await sut.SelectOrChangeContactName(new SelectOrChangeContactNameViewModel
            {
                PrimaryContact = ValidPrimaryContactSameOrganisation,
                ActionChoice = ActionChoiceSave
            });
        }

        public async Task<IActionResult> Act(SelectOrChangeContactNameViewModel viewModel)
        {
            return await sut.SelectOrChangeContactName(viewModel);
        }

        [Test]
        public async Task Should_return_a_viewresult_called_confirm()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("SelectOrChangeContactNameConfirm");
        }

    }
}
