using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Consts;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EmailValidator: AbstractValidator<EmailChecker>
    {
        public EmailValidator()
        { 
        RuleFor(x =>x.EmailToCheck)
            .NotEmpty()
            .WithMessage(EpaOrganisationValidatorMessageName.EmailIsMissing)
            .EmailAddress()
            .WithMessage(EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat);
            }
    }

    public class EmailChecker
    {
        public string EmailToCheck { get; set; }
    }
}
