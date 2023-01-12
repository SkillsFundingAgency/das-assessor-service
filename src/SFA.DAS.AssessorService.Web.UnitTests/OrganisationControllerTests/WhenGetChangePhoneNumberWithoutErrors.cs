using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangePhoneNumberWithoutErrors
        : OrganisationControllerTestBaseForModel<ChangePhoneNumberViewModel>
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
            return await sut.ChangePhoneNumber();
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
