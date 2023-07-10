using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Standard
{
    [TestFixture]
    public class AddStandardViewModelValidatorTests
    {
        private AddStandardSearchViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new AddStandardSearchViewModelValidator();
        }

        [Test]
        public void Validate_StandardToFindIsEmpty_ShouldHaveError()
        {
            var viewModel = new AddStandardSearchViewModel { Search = string.Empty };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.Search);
        }

        [Test]
        public void Validate_StandardToFindIsLessThan3Characters_ShouldHaveError()
        {
            var viewModel = new AddStandardSearchViewModel { Search = "ab" };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.Search);
        }

        [Test]
        public void Validate_StandardToFindIs3Characters_ShouldNotHaveError()
        {
            var viewModel = new AddStandardSearchViewModel { Search = "abc" };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.Search);
        }

        [Test]
        public void Validate_StandardToFindIsMoreThan3Characters_ShouldNotHaveError()
        {
            var viewModel = new AddStandardSearchViewModel { Search = "abcd" };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.Search);
        }
    }

}
