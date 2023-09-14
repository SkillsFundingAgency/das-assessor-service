using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Apply
{
    [TestFixture]
    public class ApplyStandardConfirmViewModelValidatorTests
    {
        private ApplyStandardConfirmViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new ApplyStandardConfirmViewModelValidator();
        }

        [Test]
        public void Validate_IsConfirmedIsNotChecked_ShouldHaveError()
        {
            var viewModel = new ApplyStandardConfirmViewModel { IsConfirmed = false };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.IsConfirmed);
        }

        [Test]
        public void Validate_IsConfirmedIsChecked_ShouldNotHaveError()
        {
            var viewModel = new ApplyStandardConfirmViewModel { IsConfirmed = true };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.IsConfirmed);
        }

        [Test]
        public void Validate_StandardVersionsAreNotSelected_ShouldHaveError()
        {
            var viewModel = new ApplyStandardConfirmViewModel { SelectedVersions = new List<string>() };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.SelectedVersions);
        }

        [Test]
        public void Validate_StandardVersionsAreSelected_ShouldNotHaveError()
        {
            var viewModel = new ApplyStandardConfirmViewModel { SelectedVersions = new List<string> { "1.0" } };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.SelectedVersions);
        }
    }

}
