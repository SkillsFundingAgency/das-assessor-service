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
    public class WhenGetChangeEmailWithoutErrors
        : OrganisationControllerTestBaseForModel<ChangeEmailViewModel>
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);
        }

        public override async Task<IActionResult> Act()
        {
            return await sut.ChangeEmail();
        }

        [Test]
        public async Task Should_return_a_model_with_current_email()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeEmailViewModel;

            model.Email.Should().Be(EpaOrganisation.OrganisationData?.Email);
        }
    }
}
