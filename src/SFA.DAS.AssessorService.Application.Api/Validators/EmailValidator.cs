using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EmailValidator: AbstractValidator<EmailChecker>
    {
        public EmailValidator()
        { 
            RuleFor(x =>x.EmailToCheck)
                .NotEmpty()
                .WithMessage(EpaOrganisationValidatorMessageName.EmailIsMissing)
                .EmailRegexAddress()
                .WithMessage(EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat);
        }
    }

    public class EmailChecker
    {
        public string EmailToCheck { get; set; }
    }
}
