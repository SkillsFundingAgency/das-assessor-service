using FluentValidation;
using FluentValidation.Validators;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class EmailCheckValidator : AbstractValidator<EmailCheck>
    {
            public EmailCheckValidator()
            {
                DefaultValidatorExtensions.EmailAddress(
                    RuleFor(x => x.EmailToCheck)
                    .EmailAddress(EmailValidationMode.Net4xRegex));
            }
    }
}