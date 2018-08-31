using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateRecipientViewModelValidator : AbstractValidator<CertificateRecipientViewModel>
    {
        public CertificateRecipientViewModelValidator(IStringLocalizer<CertificateRecipientViewModelValidator> localizer)
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage(localizer["NameCannotBeEmpty"]);
        }
    }
}