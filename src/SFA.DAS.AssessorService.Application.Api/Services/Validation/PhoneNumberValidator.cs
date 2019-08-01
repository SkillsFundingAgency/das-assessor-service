using FluentValidation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class PhoneNumberValidator : AbstractValidator<PhoneNumberCheck>
    {
        public PhoneNumberValidator()
        {
            DefaultValidatorExtensions.Matches<PhoneNumberCheck>(RuleFor(x => x.PhoneNumberToCheck), @"^[0-9]{10}$|^[0-9]{11}$");
        }
    }
}