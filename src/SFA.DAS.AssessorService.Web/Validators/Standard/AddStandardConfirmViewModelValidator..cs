using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardConfirmViewModelValidator : AbstractValidator<AddStandardConfirmViewModel>
    {
        public AddStandardConfirmViewModelValidator()
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