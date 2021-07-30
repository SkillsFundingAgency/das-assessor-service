using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateVersionViewModelValidator : AbstractValidator<CertificateVersionViewModel>
    {
        public CertificateVersionViewModelValidator(IStringLocalizer<CertificateVersionViewModelValidator> localizer)
        {
            RuleFor(vm => vm.StandardUId).NotEmpty().WithMessage(localizer["VersionRequired"]);
        }
    }
}