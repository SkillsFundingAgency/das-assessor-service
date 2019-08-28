using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetSelectOrChangeContactNameWithErrors
        : OrganisationControllerTestBaseForModel<SelectOrChangeContactNameViewModel>
    {
        protected string InvalidPrimaryContact = "invalid@invalid.com";

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
        }

        public override async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(SelectOrChangeContactNameViewModel.PrimaryContact), "An error message");
            sut.ModelState[nameof(SelectOrChangeContactNameViewModel.PrimaryContact)].AttemptedValue = InvalidPrimaryContact;

            return await sut.SelectOrChangeContactName();
        }

        [Test]
        public async Task Should_return_a_model_with_invalid_primary_contact()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as SelectOrChangeContactNameViewModel;

            // the primary contact is replaced with the invalid one to be replayed in the view
            model.PrimaryContact.Should().Be(InvalidPrimaryContact);
        }
    }
}
