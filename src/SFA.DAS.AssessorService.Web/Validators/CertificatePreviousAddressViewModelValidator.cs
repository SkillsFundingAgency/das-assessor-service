using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificatePreviousAddressViewModelValidator : AbstractValidator<CertificatePreviousAddressViewModel>
    {
        public CertificatePreviousAddressViewModelValidator(IStringLocalizer<CertificatePreviousAddressViewModelValidator> localizer)
        {
            RuleFor(vm => vm.UsePreviousAddress).NotNull().WithMessage(localizer["UsePreviousAddress"]);
        }
    }
}