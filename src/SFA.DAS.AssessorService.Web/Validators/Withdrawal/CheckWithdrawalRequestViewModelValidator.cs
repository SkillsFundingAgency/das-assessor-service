using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class CheckWithdrawalRequestViewModelValidator : AbstractValidator<CheckWithdrawalRequestViewModel>
    {
        public CheckWithdrawalRequestViewModelValidator()
        {
            RuleFor(vm => vm.Continue)
                .Must(value => !string.IsNullOrEmpty(value) && (value.ToLower() == "yes" || value.ToLower() == "no"))
                .WithMessage("Select Yes or No");
        }
    }
}