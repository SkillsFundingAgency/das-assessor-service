using FluentValidation;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class SetSettingRequestValidator : AbstractValidator<SetSettingRequest>
    {
        public SetSettingRequestValidator(IStringLocalizer<SetSettingRequestValidator> localiser)
        {
            const int MaximumNameLength = 50;
            const int MaximumValueLength = 256;

            RuleFor(m => m.Name.Length).InclusiveBetween(1, MaximumNameLength).WithMessage($"The name of a setting must be between 1 and {MaximumNameLength} characters");
            RuleFor(m => m.Value.Length).InclusiveBetween(1, MaximumValueLength).WithMessage($"The value of a setting must be between 1 and {MaximumValueLength} characters");
        }
    }
}
