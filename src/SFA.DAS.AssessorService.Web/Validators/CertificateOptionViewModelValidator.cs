using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateOptionViewModelValidator : AbstractValidator<CertificateOptionViewModel>
    {
        public CertificateOptionViewModelValidator(IStringLocalizer<CertificateOptionViewModelValidator> localizer)
        {
            RuleFor(vm => vm.Option).NotEmpty().WithMessage(localizer["OptionRequired"]);
        }
    }
}