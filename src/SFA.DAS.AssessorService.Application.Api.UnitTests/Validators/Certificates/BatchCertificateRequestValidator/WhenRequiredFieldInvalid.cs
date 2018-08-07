using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.BatchCertificateRequestValidator
{
    public class WhenRequiredFieldInvalid : BatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            BatchCertificateRequest request = Builder<BatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1)
                .With(i => i.StandardCode = 1)
                .With(i => i.UkPrn = 10000000)
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                                .With(cd => cd.ContactPostCode = "BAD VALUE")
                                .With(cd => cd.AchievementDate = null)
                                .Build())
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
