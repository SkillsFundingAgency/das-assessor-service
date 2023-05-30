using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardViewModelValidator : AbstractValidator<AddStandardViewModel>
    {
        public AddStandardViewModelValidator()
        {
            const string searchMessage = "Enter 3 or more characters to search";
            RuleFor(vm => vm.StandardToFind)
                .NotEmpty().WithMessage(searchMessage)
                .MinimumLength(3).WithMessage(searchMessage);
        }
    }
}