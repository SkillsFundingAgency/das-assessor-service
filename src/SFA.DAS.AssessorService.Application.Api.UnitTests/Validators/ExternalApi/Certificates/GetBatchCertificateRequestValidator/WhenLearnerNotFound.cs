﻿using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator
{
    public class WhenLearnerNotFound : GetBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            GetBatchCertificateRequest request = Builder<GetBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567899)
                .With(i => i.StandardCode = 99)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validationResult.Errors.First().ErrorMessage.Should().Be("Cannot find apprentice with the specified Uln, FamilyName & Standard");
        }
    }
}
