using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Withdrawal
{
    [TestFixture]
    public class CheckWithdrawalRequestViewModelValidatorTests
    {
        private CheckWithdrawalRequestViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new CheckWithdrawalRequestViewModelValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void Validate_ContinueIsNotChoosen_ShouldHaveError(string selection)
        {
            var viewModel = new CheckWithdrawalRequestViewModel { Continue = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.Continue);
        }

        [TestCase("maybe")]
        [TestCase("true")]
        [TestCase("false")]
        public void Validate_ContinueIsInvalid_ShouldHaveError(string selection)
        {
            var viewModel = new CheckWithdrawalRequestViewModel { Continue = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.Continue);
        }

        [TestCase("no")]
        [TestCase("yes")]
        public void Validate_ContinueIsChoosen_ShouldNotHaveError(string selection)
        {
            var viewModel = new CheckWithdrawalRequestViewModel { Continue = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.Continue);
        }
    }
}
