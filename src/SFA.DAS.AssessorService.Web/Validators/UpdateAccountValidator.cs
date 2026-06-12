using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class UpdateAccountValidator : AbstractValidator<AccountViewModel>
    {
        public UpdateAccountValidator()
        {
            RuleFor(vm => vm.FamilyName)
                .NotEmpty().WithMessage("Family name must not be empty");

            RuleFor(vm => vm.GivenName)
                .NotEmpty().WithMessage("Given name must not be empty");
        }
    }
}

