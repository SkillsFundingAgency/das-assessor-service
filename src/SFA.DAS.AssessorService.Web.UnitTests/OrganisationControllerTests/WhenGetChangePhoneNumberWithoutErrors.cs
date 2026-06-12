using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangePhoneNumberWithoutErrors : OrganisationControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangePhoneNumber();
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
            result.Model.Should().BeOfType<ChangePhoneNumberViewModel>();
        }

        [Test]
        public async Task Should_return_a_model_with_current_phone_number()
        {
            _actionResult = await Act();
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangePhoneNumberViewModel;

            model.PhoneNumber.Should().Be(EpaOrganisation.OrganisationData?.PhoneNumber);
        }
    }
}
