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
                .Must(value => !string.IsNullOrEmpty(value) && (value == ApplicationTypes.StandardWithdrawal || value == ApplicationTypes.OrganisationWithdrawal))
                .WithMessage("Select standard or register");
        }
    }
}