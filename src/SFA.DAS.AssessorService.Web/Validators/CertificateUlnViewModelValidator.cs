using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.Validators
{
    public class CertificateUlnViewModelValidator : AbstractValidator<CertificateUlnViewModel>
    {
        public CertificateUlnViewModelValidator(IStringLocalizer<CertificateUlnViewModelValidator> localizer)
        {
            RuleFor(x => x.Uln)
                .NotEmpty().WithMessage(localizer["UlnNameCannotBeEmpty"])
                .Matches(@"^\d{10}$").WithMessage(localizer["UlnMatches"]);
        }
    }
}
