﻿using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.DeleteBatchEpaRequestValidator
{
    public class WhenValidatorValidatesSuccessfully : DeleteBatchEpaRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            var request = Builder<DeleteBatchEpaRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 101)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .With(i => i.EpaReference = "1234567890-101")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeTrue()
        {
            _validationResult.IsValid.Should().BeTrue();
        }
    }
}
