using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateSendToViewModelValidator : AbstractValidator<CertificateSendToViewModel>
    {
        public CertificateSendToViewModelValidator(IStringLocalizer<CertificateSendToViewModelValidator> localizer)
        {
            RuleFor(vm => vm.SendTo).NotEmpty().WithMessage(localizer["SendTo"]);
        }
    }
}