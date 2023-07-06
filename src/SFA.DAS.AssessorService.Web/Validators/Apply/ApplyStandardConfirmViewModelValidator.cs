using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class ApplyStandardConfirmViewModelValidator : AbstractValidator<ApplyStandardConfirmViewModel>
    {
        public ApplyStandardConfirmViewModelValidator()
        {
            RuleFor(vm => vm.IsConfirmed)
                .NotEmpty()
                .WithMessage("Confirm you have read the assessment plan");

            RuleFor(vm => vm.SelectedVersions)
                .NotEmpty()
                .WithMessage("You must select at least one version");
        }
    }
}