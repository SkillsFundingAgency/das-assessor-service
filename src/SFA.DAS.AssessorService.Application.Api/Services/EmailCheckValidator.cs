using FluentValidation;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class EmailCheckValidator : AbstractValidator<EmailCheck>
    {
            public EmailCheckValidator()
            {
                RuleFor(x => x.EmailToCheck)
                .EmailAddress();
            }
    }
}