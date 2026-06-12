using FluentValidation;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class EmailCheckValidator : AbstractValidator<EmailCheck>
    {
            public EmailCheckValidator()
            {
                DefaultValidatorExtensions.EmailAddress(
                    RuleFor(x => x.EmailToCheck)
                    .EmailRegexAddress());
            }
    }
}