﻿namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.UkPrnValidator
{
    using FluentValidation.Results;
    using Machine.Specifications;
    using FluentAssertions;
    using NUnit.Framework;

    [Subject("AssessorService")]
    public class WhenUkPrnValidatorValidatesSuccesfully : UkPrnValidatorTestBase
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
