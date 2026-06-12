using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenPostChangePhoneNumberSaveWithoutErrors : OrganisationControllerTestBase
    {
        private const string ValidPhoneNumberDifferent = "9876543210";
        private const string ActionChoiceSave = "Save";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false,
                contactsPrivileges: null);

            ValidateApiClient.Setup(c => c.ValidatePhoneNumber(ValidPhoneNumberDifferent)).ReturnsAsync(true);
        }

        public async Task<IActionResult> Act()
        {
            return await sut.ChangePhoneNumber(new ChangePhoneNumberViewModel
            {
                PhoneNumber = ValidPhoneNumberDifferent,
                ActionChoice = ActionChoiceSave
            });
        }

        public async Task<IActionResult> Act(ChangePhoneNumberViewModel viewModel)
        {
            return await sut.ChangePhoneNumber(viewModel);
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
        public async Task Should_return_a_viewresult_called_confirm()
        {
            _actionResult = await Act();
            var viewResult = _actionResult as ViewResult;
            viewResult.ViewName.Should().Be("ChangePhoneNumberConfirm");
        }
    }
}
