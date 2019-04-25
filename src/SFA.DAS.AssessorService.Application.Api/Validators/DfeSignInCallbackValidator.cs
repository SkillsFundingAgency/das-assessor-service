using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class DfeSignInCallbackValidator : AbstractValidator<DfeSignInCallback>
    {
        public DfeSignInCallbackValidator(IStringLocalizer<CreateContactRequest> localizer)
        {
            RuleFor(m => m.Sub)
                .NotEmpty().WithMessage(localizer["Sub must not be empty"])
                .Must(m => Guid.TryParse(m, out _)).WithMessage("Sub must be a Guid");

            RuleFor(m => m.SourceId)
                .NotEmpty().WithMessage(localizer["SourceId must not be empty"])
                .Must(m => Guid.TryParse(m, out _)).WithMessage("SourceId must be a Guid");
        }
    }
}
