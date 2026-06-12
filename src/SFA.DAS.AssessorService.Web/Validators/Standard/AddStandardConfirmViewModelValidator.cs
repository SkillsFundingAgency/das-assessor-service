using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardConfirmViewModelValidator : AbstractValidator<AddStandardConfirmViewModel>
    {
        public AddStandardConfirmViewModelValidator()
        {
            RuleFor(vm => vm.IsConfirmed)
                .NotEqual(false)
                .WithMessage("Confirm you have read the assessment plan");

            RuleFor(vm => vm.SelectedVersions)
                .NotEmpty()
                .WithMessage("You must select at least one version");
        }
    }
}