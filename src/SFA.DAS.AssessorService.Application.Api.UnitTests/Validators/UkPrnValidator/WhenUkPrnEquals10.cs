namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using FluentAssertions;

    [Subject("AssessorService")]
    public class WhenUkPrnEquals10 : UkPrnValidatorTestBase
    {
        private static ValidationResult _validationResult;

        Establish context = () =>
        {
            Setup();
        };

        Because of = () =>
        {
            _validationResult = UkPrnValidator.Validate(10);
        };

        Machine.Specifications.It verify_succesfully = () =>
        {
            _validationResult.IsValid.Should().BeFalse();
        };
    }
}
