using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateOptionViewModelValidator : AbstractValidator<CertificateOptionViewModel>
    {
        public CertificateOptionViewModelValidator(IStringLocalizer<CertificateOptionViewModelValidator> localizer)
        {
            //RuleFor(vm => vm).Custom((vm, context) =>
            //{
            //    if (vm.HasAdditionalLearningOption == null)
            //    {
            //        context.AddFailure("HasAdditionalLearningOption", localizer["YesOrNoRequired"]);
            //    }

            //    if (vm.HasAdditionalLearningOption.HasValue && vm.HasAdditionalLearningOption.Value == true && string.IsNullOrWhiteSpace(vm.Option))
            //    {
            //        context.AddFailure("Option", localizer["OptionRequired"]);
            //    }
            //});
        }
    }
}