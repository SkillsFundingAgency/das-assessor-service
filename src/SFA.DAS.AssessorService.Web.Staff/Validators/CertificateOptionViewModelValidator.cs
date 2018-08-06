using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class CertificateOptionViewModelValidator : AbstractValidator<CertificateOptionViewModel>
    {
        public CertificateOptionViewModelValidator(IStringLocalizer<CertificateOptionViewModelValidator> localizer)
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.HasAdditionalLearningOption == null)
                {
                    context.AddFailure("HasAdditionalLearningOption", "Select yes or no");
                }

                if (vm.HasAdditionalLearningOption.HasValue && vm.HasAdditionalLearningOption.Value == true && string.IsNullOrWhiteSpace(vm.Option))
                {
                    context.AddFailure("Option", "Enter the learning option");
                }
            });
        }
    }
}