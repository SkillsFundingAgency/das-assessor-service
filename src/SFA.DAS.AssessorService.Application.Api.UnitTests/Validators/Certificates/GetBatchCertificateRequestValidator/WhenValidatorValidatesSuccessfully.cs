using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.GetBatchCertificateRequestValidator
{
    public class WhenValidatorValidatesSuccessfully : GetBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            GetBatchCertificateRequest request = Builder<GetBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 1)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .Build();

            _validationResult = Validator.Validate(request);
        }

        [Test]
        public void ThenValidationResultShouldBeTrue()
        {
            _validationResult.IsValid.Should().BeTrue();
        }
    }
}
