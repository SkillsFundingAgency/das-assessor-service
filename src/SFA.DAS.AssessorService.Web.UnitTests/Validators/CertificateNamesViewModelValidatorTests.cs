using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators
{
    [TestFixture]
    public class CertificateNamesViewModelValidatorTests
    {
        private CertificateNamesViewModel _viewModel;
        private CertificateBaseViewModel _certificateBaseViewModel;
        private CertificateNameViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _certificateBaseViewModel = new Mock<CertificateBaseViewModel>().Object;
            _viewModel = CreateValidViewModel(_certificateBaseViewModel);

            _validator = new CertificateNameViewModelValidator();
        }

        [Test, MoqAutoData]
        public void WhenInputIsValid_ThenReturnsValid(CertificateBaseViewModel _baseViewModel)
        {
            _viewModel = CreateValidViewModel(_baseViewModel);

            var result = _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);
        }

        [Test, MoqAutoData]
        public void WhenGivenNamesFieldIsEmpty_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            _viewModel = CreateValidViewModel(_baseViewModel);
            _viewModel.GivenNames = string.Empty;

            var result = _validator.TestValidate(_viewModel);

            result.IsValid.Should().Be(false);
            result.ShouldHaveValidationErrorFor(x => x.InputGivenNames);
        }

        [Test, MoqAutoData]
        public void WhenFamilyNameFieldIsEmpty_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            _viewModel = CreateValidViewModel(_baseViewModel);
            _viewModel.FamilyName = string.Empty;

            var result = _validator.TestValidate(_viewModel);

            result.IsValid.Should().Be(false);
            result.ShouldHaveValidationErrorFor(x => x.InputFamilyName);
        }

        [Test, MoqAutoData]
        public void WhenGivenNamesFieldIsNotEqualToPreviousGivenNamesValue_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            _viewModel = CreateValidViewModel(_baseViewModel);
            _viewModel.GivenNames = "random given names";

            var result = _validator.TestValidate(_viewModel);

            result.IsValid.Should().Be(false);
            result.ShouldHaveValidationErrorFor(x => x.InputGivenNames);
        }

        [Test, MoqAutoData]
        public void WhenFamilyNameFieldIsNotEqualToPreviousFamilyNameValue_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            _viewModel = CreateValidViewModel(_baseViewModel);
            _viewModel.FamilyName = "random family name";

            var result = _validator.TestValidate(_viewModel);

            result.IsValid.Should().Be(false);
            result.ShouldHaveValidationErrorFor(x => x.InputFamilyName);
        }


        private CertificateNamesViewModel CreateValidViewModel(CertificateBaseViewModel _baseViewModel)
        {
            return new CertificateNamesViewModel() { InputGivenNames = _baseViewModel.GivenNames, InputFamilyName = _baseViewModel.FamilyName, GivenNames = _baseViewModel.GivenNames, FamilyName = _baseViewModel.FamilyName };
        }

    }
}
