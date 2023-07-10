using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardSearchViewModelValidator : AbstractValidator<AddStandardSearchViewModel>
    {
        public AddStandardSearchViewModelValidator()
        {
            const string searchMessage = "Enter 3 or more characters to search";
            RuleFor(vm => vm.Search)
                .NotEmpty().WithMessage(searchMessage)
                .MinimumLength(3).WithMessage(searchMessage);
        }
    }
}