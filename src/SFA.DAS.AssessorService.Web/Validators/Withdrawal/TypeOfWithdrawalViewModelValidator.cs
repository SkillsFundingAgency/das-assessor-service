using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class TypeOfWithdrawalViewModelValidator : AbstractValidator<TypeOfWithdrawalViewModel>
    {
        public TypeOfWithdrawalViewModelValidator()
        {
            RuleFor(vm => vm.TypeOfWithdrawal)
                .NotEmpty()
                .WithMessage("Select standard or register");
        }
    }
}