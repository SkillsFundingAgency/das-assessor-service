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

        [TestCase("")]
        [TestCase("a")]
        [TestCase("ab")]
        [TestCase(" ab")]
        [TestCase("ab ")]
        [TestCase("   ")]
        [TestCase("    ")]
        public void Validate_Search_ShouldHaveError(string search)
        {
            var viewModel = new AddStandardSearchViewModel { Search = search };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.Search);
        }

        [TestCase("abcd")]
        [TestCase("abc ")]
        [TestCase(" abc")]
        public void Validate_Search_ShouldNotHaveError(string search)
        {
            var viewModel = new AddStandardSearchViewModel { Search = search };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.Search);
        }
    }
}
