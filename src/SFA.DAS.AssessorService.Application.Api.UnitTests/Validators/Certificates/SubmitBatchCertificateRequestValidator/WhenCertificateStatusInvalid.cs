using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.SubmitBatchCertificateRequestValidator
{
    public class WhenCertificateStatusInvalid : SubmitBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            long uln = 12345;
            int standardCode = 54321;
            string certificateReference = "submit-status-invalid-test";
            string status = "Printed";

            AddMockCertificate(uln, standardCode, certificateReference, status);

            SubmitBatchCertificateRequest request = Builder<SubmitBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.UkPrn = 10000000)
                .Build();

            _validationResult = Validator.Validate(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
        }
    }


}
