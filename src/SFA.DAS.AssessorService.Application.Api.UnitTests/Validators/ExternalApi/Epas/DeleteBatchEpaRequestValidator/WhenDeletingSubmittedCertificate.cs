using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Epas.DeleteBatchEpaRequestValidator
{
    public class WhenDeletingSubmittedCertificate : DeleteBatchEpaRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            var request = Builder<DeleteBatchEpaRequest>.CreateNew()
                .With(i => i.Uln = 9999999999)
                .With(i => i.StandardCode = 1)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 99999999)
                .With(i => i.FamilyName = "Test")
                .With(i => i.EpaReference = "9999999999-1")
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validationResult.Errors.Count.Should().Be(1);
        }
    }


}
