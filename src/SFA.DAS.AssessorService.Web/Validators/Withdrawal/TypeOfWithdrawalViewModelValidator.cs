using FluentValidation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class TypeOfWithdrawalViewModelValidator : AbstractValidator<TypeOfWithdrawalViewModel>
    {
        public TypeOfWithdrawalViewModelValidator()
        {
            RuleFor(vm => vm.TypeOfWithdrawal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(t => t == ApplicationTypes.StandardWithdrawal || t == ApplicationTypes.OrganisationWithdrawal)
                .WithMessage("Select standard or register");
        }
    }
}