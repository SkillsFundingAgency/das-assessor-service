using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class CheckWithdrawalRequestViewModelValidator : AbstractValidator<CheckWithdrawalRequestViewModel>
    {
        public CheckWithdrawalRequestViewModelValidator()
        {
            RuleFor(vm => vm.Continue)
                .NotEmpty()
                .WithMessage("Select Yes or No");
        }
    }
}