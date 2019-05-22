using FluentValidation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UkrlpNotFoundViewModelValidator : AbstractValidator<UkrlpNotFoundViewModel>
    {
        public UkrlpNotFoundViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (!string.IsNullOrEmpty(vm?.NextAction) || !string.IsNullOrEmpty(vm?.FirstEntry)) return;

                context.AddFailure("NextAction", "Tell us what you want to do");
            });
        }

    }
}