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
    public class WhenGetChangeEmailWithErrors : OrganisationControllerTestBase
    {
        private const string InvalidEmail = "NOTANEMAILADDRESS";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false);
        }

        public async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(ChangeEmailViewModel.Email), "An error message");
            sut.ModelState[nameof(ChangeEmailViewModel.Email)].AttemptedValue = InvalidEmail;

            return await sut.ChangeEmail();
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
            result.Model.Should().BeOfType<ChangeEmailViewModel>();
        }

        [Test]
        public async Task Should_return_a_model_with_invalid_email()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeEmailViewModel;

            // the email is replaced with the invalid one to be replayed in the view
            model.Email.Should().Be(InvalidEmail);
        }
    }
}
