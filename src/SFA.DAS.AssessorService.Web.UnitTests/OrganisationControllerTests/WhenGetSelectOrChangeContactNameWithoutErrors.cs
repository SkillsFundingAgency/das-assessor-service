﻿using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetSelectOrChangeContactNameWithoutErrors : OrganisationControllerTestBase
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

        public async Task<IActionResult> Act()
        {
            return await sut.SelectOrChangeContactName();
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
            result.Model.Should().BeOfType<SelectOrChangeContactNameViewModel>();
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
