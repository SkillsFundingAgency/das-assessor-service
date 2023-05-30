using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Validators.Standard
{
    public class AddStandardConfirmViewModelValidator : AbstractValidator<AddStandardConfirmViewModel>
    {
        public AddStandardConfirmViewModelValidator(IStandardVersionClient standardVersionApiClient)
        {
            RuleFor(vm => vm.IsConfirmed)
                .NotEmpty()
                .WithMessage("Confirm you have read the assessment plan");

            RuleFor(vm => vm)
                .Custom((vm, context) =>
                {
                    var standardVersions = standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(vm.StandardReference).Result;
                    if (standardVersions.Count() > 1 && !(vm.SelectedVersions?.Any() ?? false))
                    {
                        context.AddFailure(new ValidationFailure(nameof(vm.SelectedVersions), "You must select at least one version"));
                    }
                });
        }
    }
}