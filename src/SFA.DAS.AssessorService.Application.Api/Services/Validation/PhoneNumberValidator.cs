using FluentValidation;

namespace SFA.DAS.AssessorService.Application.Api.Services.Validation
{
    public class PhoneNumberValidator : AbstractValidator<PhoneNumberCheck>
    {
        public PhoneNumberValidator()
        {
            string validUkPhoneNumberRegEx = @"^([+]{0,1}[/0-9]{1,4})[\s]*[(]{0,1}[0-9]{1,5}[)]{0,1}[-\s\./0-9]*$";
            DefaultValidatorExtensions.Matches<PhoneNumberCheck>(RuleFor(x => x.PhoneNumberToCheck), validUkPhoneNumberRegEx);
        }
    }
}