using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.Batch
{
    public class WhenCertificateExists : BatchCertificateRequestValidatorBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            BatchCertificateRequest request = Builder<BatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = Uln_Certificate)
                .With(i => i.StandardCode = StandardCode_Certificate)
                .With(i => i.UkPrn = 10000000)
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                                .With(cd => cd.ContactPostCode = "AA11AA")
                                .With(cd => cd.AchievementDate = DateTime.UtcNow)
                                .Build())
                .Build();

            _validationResult = BatchCertificateRequestValidator.Validate(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
        }
    }
}
