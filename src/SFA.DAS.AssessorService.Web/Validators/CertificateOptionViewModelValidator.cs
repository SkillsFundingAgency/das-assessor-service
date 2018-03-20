using FluentValidation;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateOptionViewModelValidator : AbstractValidator<CertificateOptionViewModel>
    {
        public CertificateOptionViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (vm.HasAdditionalLearningOption == null)
                {
                    context.AddFailure("HasAdditionalLearningOption", "Yes or No must be selected.");
                }

                if (vm.HasAdditionalLearningOption.HasValue && vm.HasAdditionalLearningOption.Value == true && string.IsNullOrWhiteSpace(vm.Option))
                {
                    context.AddFailure("Option", "Please specify the Option.");
                }
            });
        }
    }
}