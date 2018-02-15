namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;

    [Subject("AssessorService")]
    public class WhenUkPnEquals100000001
    {
        private static UkPrnValidator _ukPrnValidator;
        private static ValidationResult _validationResult;

        Establish context = () =>
        {
            _ukPrnValidator = new UkPrnValidator();
        };

        Because of = () =>
        {
            _validationResult = _ukPrnValidator.Validate(100000001);
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };
    }
}
