using FluentValidation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class EmailCheckValidator : AbstractValidator<EmailCheck>
    {
            public EmailCheckValidator()
            {
                DefaultValidatorExtensions.EmailAddress<EmailCheck>(RuleFor(x => x.EmailToCheck));
            }
    }
}