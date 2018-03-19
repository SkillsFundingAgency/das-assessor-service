using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class StartCertificateRequestValidator : AbstractValidator<StartCertificateRequest>
    {
        public StartCertificateRequestValidator(IStringLocalizer<StartCertificateRequestValidator> localizer)
        {
            RuleFor(r => r.UkPrn).NotEmpty().WithMessage("Ukprn must not be empty");
            RuleFor(r => r.Uln).NotEmpty().WithMessage("Uln must not be empty");
            RuleFor(r => r.StandardCode).NotEmpty().WithMessage("Standard Code must not be empty");
            RuleFor(r => r.Username).NotEmpty().WithMessage("Username must not be empty");
        }
    }
}