using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Withdrawal
{
    [TestFixture]
    public class TypeOfWithdrawalViewModelValidatorTests
    {
        private TypeOfWithdrawalViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new TypeOfWithdrawalViewModelValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void Validate_ContinueIsNotChoosen_ShouldHaveError(string selection)
        {
            var viewModel = new TypeOfWithdrawalViewModel { TypeOfWithdrawal = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.TypeOfWithdrawal);
        }

        [TestCase("Some")]
        [TestCase("Other")]
        [TestCase("Things")]
        public void Validate_ContinueIsInvalid_ShouldHaveError(string selection)
        {
            var viewModel = new TypeOfWithdrawalViewModel { TypeOfWithdrawal = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldHaveValidationErrorFor(vm => vm.TypeOfWithdrawal);
        }

        [TestCase("StandardWithdrawal")]
        [TestCase("OrganisationWithdrawal")]
        public void Validate_ContinueIsChoosen_ShouldNotHaveError(string selection)
        {
            var viewModel = new TypeOfWithdrawalViewModel { TypeOfWithdrawal = selection };

            var result = _validator.TestValidate(viewModel);

            result.ShouldNotHaveValidationErrorFor(vm => vm.TypeOfWithdrawal);
        }
    }
}
