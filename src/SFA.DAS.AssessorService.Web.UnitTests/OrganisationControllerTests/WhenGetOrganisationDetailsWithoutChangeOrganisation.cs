using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetOrganisationDetailsWithoutChangeOrganisation
        : OrganisationControllerTestBaseForModel<ViewAndEditOrganisationViewModel>
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                contactsPrivileges: new List<ContactsPrivilege>
                {
                    // the contact has the explict 'Manage Users' privilege
                    ManageUsersContactsPrivilege
                });
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.OrganisationDetails();
        }

        [Test]
        public async Task Should_return_a_model_with_access_denied_and_manage_users()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ViewAndEditOrganisationViewModel;

            model.UserHasChangeOrganisationPrivilege.Should().BeFalse();
            model.AccessDeniedViewModel.Should().NotBeNull();
            model.AccessDeniedViewModel.PrivilegeId.Should().Be(ChangeOrganisationPrivilegeId);
            model.AccessDeniedViewModel.UserHasUserManagement.Should().BeTrue();
            model.AccessDeniedViewModel.ReturnController = nameof(OrganisationController).RemoveController();
            model.AccessDeniedViewModel.ReturnAction = nameof(OrganisationController.OrganisationDetails);
        }
    }
}
