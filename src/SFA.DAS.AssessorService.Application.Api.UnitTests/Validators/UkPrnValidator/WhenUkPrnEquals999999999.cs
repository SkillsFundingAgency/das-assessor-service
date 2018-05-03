using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    public class WhenUkPrnValidatorValidatorUkPrnEquals99999999 : UkPrnValidatorTestBase
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _validationResult = UkPrnValidator.Validate(99999999);
        }

        [Test]
        public void ThenTheRepositoryIsAskedToDeleteTheCorrectOrganisation()
        {
            _validationResult.IsValid.Should().BeTrue();
        }
    }
}