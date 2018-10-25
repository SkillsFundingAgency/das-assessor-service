using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateLastNameViewModelValidator : AbstractValidator<CertificateLastNameViewModel>
    {
        public CertificateLastNameViewModelValidator(IStringLocalizer<CertificateLastNameViewModelValidator> localizer)
        {
            RuleFor(vm => vm.LastName).NotEmpty().WithMessage(localizer["LastNameCannotBeEmpty"]);
        }
    }
}
