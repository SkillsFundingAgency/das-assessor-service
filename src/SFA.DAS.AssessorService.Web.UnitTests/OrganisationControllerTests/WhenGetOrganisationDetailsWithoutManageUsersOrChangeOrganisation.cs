using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetOrganisationDetailsWithoutManageUsersOrChangeOrganisation
        : OrganisationControllerTestBaseForModel<ViewAndEditOrganisationViewModel>
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true,
                addUkprnClaim: false,
                // the contact has only the implicit 'View Dashboard' privilege
                contactsPrivileges: new List<ContactsPrivilege>());
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.OrganisationDetails();
        }

        [Test]
        public async Task Should_return_a_model_with_access_denied()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ViewAndEditOrganisationViewModel;

            model.UserHasChangeOrganisationPrivilege.Should().BeFalse();
            model.AccessDeniedViewModel.Should().NotBeNull();
            model.AccessDeniedViewModel.PrivilegeId.Should().Be(ChangeOrganisationPrivilegeId);
            model.AccessDeniedViewModel.UserHasUserManagement.Should().BeFalse();
            model.AccessDeniedViewModel.ReturnController = nameof(OrganisationController).RemoveController();
            model.AccessDeniedViewModel.ReturnAction = nameof(OrganisationController.OrganisationDetails);
        }
    }
}
