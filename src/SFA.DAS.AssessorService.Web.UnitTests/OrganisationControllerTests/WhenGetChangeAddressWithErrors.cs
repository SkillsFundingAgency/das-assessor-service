using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

using SFA.DAS.AssessorService.Web.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    [TestFixture]
    public class WhenGetChangeAddressWithErrors : OrganisationControllerTestBase
    {
        private const string InvalidAddressLine1 = "";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(
                addEpaoClaim: true, 
                addUkprnClaim: false);
        }

        public async Task<IActionResult> Act()
        {
            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine1), "An error message");
            sut.ModelState[nameof(ChangeAddressViewModel.AddressLine1)].AttemptedValue = InvalidAddressLine1;

            return await sut.ChangeAddress();
        }

        public async Task<IActionResult> Act(string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postcode)
        {
            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine1), string.IsNullOrEmpty(addressLine1) ? "An error message" : string.Empty);
            sut.ModelState[nameof(ChangeAddressViewModel.AddressLine1)].AttemptedValue = addressLine1;

            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine2), string.IsNullOrEmpty(addressLine2) ? "An error message" : string.Empty);
            sut.ModelState[nameof(ChangeAddressViewModel.AddressLine2)].AttemptedValue = addressLine2;

            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine3), string.IsNullOrEmpty(addressLine3) ? "An error message" : string.Empty);
            sut.ModelState[nameof(ChangeAddressViewModel.AddressLine3)].AttemptedValue = addressLine3;

            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine4), string.IsNullOrEmpty(addressLine4) ? "An error message" : string.Empty);
            sut.ModelState[nameof(ChangeAddressViewModel.AddressLine4)].AttemptedValue = addressLine4;

            sut.ModelState.AddModelError(nameof(ChangeAddressViewModel.Postcode), string.IsNullOrEmpty(postcode) ? "An error message" : string.Empty);
            sut.ModelState[nameof(ChangeAddressViewModel.Postcode)].AttemptedValue = postcode;

            return await sut.ChangeAddress();
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
            result.Model.Should().BeOfType<ChangeAddressViewModel>();
        }

        [TestCase("VALUE", "VALUE", "VALUE", "VALUE", "")]
        [TestCase("VALUE", "VALUE", "VALUE", "", "VALUE")]
        [TestCase("VALUE", "VALUE", "", "VALUE", "VALUE")]
        [TestCase("VALUE", "", "VALUE", "VALUE", "VALUE")]
        [TestCase("", "VALUE", "VALUE", "VALUE", "VALUE")]
        public async Task Should_return_a_model_with_invalid_address(string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postcode)
        {
            _actionResult = await Act(addressLine1, addressLine2, addressLine3, addressLine4, postcode);
            var result = _actionResult as ViewResult;
            var model = result.Model as ChangeAddressViewModel;

            // the address is replaced with the values previously entered if any address field is invalid
            model.AddressLine1.Should().Be(addressLine1);
            model.AddressLine2.Should().Be(addressLine2);
            model.AddressLine3.Should().Be(addressLine3);
            model.AddressLine4.Should().Be(addressLine4);
            model.Postcode.Should().Be(postcode);

            // the model state should only contain an error when field has an error (i.e. the field is empty)
            result.ViewData.ModelState[nameof(ChangeAddressViewModel.AddressLine1)].Errors.Should().Contain(p => p.ErrorMessage == (string.IsNullOrEmpty(addressLine1) ? "An error message" : string.Empty));
            result.ViewData.ModelState[nameof(ChangeAddressViewModel.AddressLine2)].Errors.Should().Contain(p => p.ErrorMessage == (string.IsNullOrEmpty(addressLine2) ? "An error message" : string.Empty));
            result.ViewData.ModelState[nameof(ChangeAddressViewModel.AddressLine3)].Errors.Should().Contain(p => p.ErrorMessage == (string.IsNullOrEmpty(addressLine3) ? "An error message" : string.Empty));
            result.ViewData.ModelState[nameof(ChangeAddressViewModel.AddressLine4)].Errors.Should().Contain(p => p.ErrorMessage == (string.IsNullOrEmpty(addressLine4) ? "An error message" : string.Empty));
            result.ViewData.ModelState[nameof(ChangeAddressViewModel.Postcode)].Errors.Should().Contain(p => p.ErrorMessage == (string.IsNullOrEmpty(postcode) ? "An error message" : string.Empty));
        }
    }
}
