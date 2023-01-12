using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetSelectOrChangeContactNameWithoutErrors
        : OrganisationControllerTestBaseForModel<SelectOrChangeContactNameViewModel>
    {
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
            return await sut.SelectOrChangeContactName();
        }

        [Test]
        public async Task Should_return_a_model_with_current_primary_contact()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as SelectOrChangeContactNameViewModel;

            model.PrimaryContact.Should().Be(EpaOrganisation.PrimaryContact);
        }
    }
}
