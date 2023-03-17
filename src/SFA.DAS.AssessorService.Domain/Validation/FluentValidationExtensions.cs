using FluentValidation;
using FluentValidation.Validators;

namespace SFA.DAS.AssessorService.Domain.Validation
{
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// Defines an email validator on the current rule builder for string properties.
        /// Validation will fail if the value returned by the lambda is not a valid email address which is
        /// defined by the regular expression of the original Fluent Validation EmailValidator.
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> EmailRegexAddress<T>(
          this IRuleBuilder<T, string> ruleBuilder)
        {
            PropertyValidator<T, string> propertyValidator = new EmailRegexValidator<T>();
            return ruleBuilder.SetValidator(propertyValidator);
        }
    }
}
