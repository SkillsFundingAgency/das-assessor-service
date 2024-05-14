using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class DfeSignInCallbackValidator : AbstractValidator<SignInCallback>
    {
        public DfeSignInCallbackValidator(IStringLocalizer<CreateContactRequest> localizer)
        {
            RuleFor(m => m.GovIdentifier)
                .NotEmpty().WithMessage(localizer["GovUkIdentifier must not be empty"]);

            RuleFor(m => m.SourceId)
                .NotEmpty().WithMessage(localizer["SourceId must not be empty"])
                .Must(m => Guid.TryParse(m, out _)).WithMessage("SourceId must be a Guid");
        }
    }
}
