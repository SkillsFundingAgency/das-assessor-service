using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.SubmitBatchCertificateRequestValidator
{
    public class WhenValidatorValidatesSuccessfully : SubmitBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            long uln = 12345;
            int standardCode = 12345;
            string certificateReference = "12345";
            string status = "Ready";

            AddMockCertificate(uln, standardCode, certificateReference, status);

            SubmitBatchCertificateRequest request = Builder<SubmitBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.UkPrn = 10000000)
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
