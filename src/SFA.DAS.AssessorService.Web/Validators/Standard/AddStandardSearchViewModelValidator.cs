using FluentValidation;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardSearchViewModelValidator : AbstractValidator<AddStandardSearchViewModel>
    {
        public AddStandardSearchViewModelValidator()
        {
            RuleFor(vm => vm.Search)
                .Must((vm, search) => !string.IsNullOrWhiteSpace(search) && search.Trim().Length >= 3)
                .WithMessage("Enter 3 or more characters to search");
        }
    }
}