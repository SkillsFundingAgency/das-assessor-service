using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.UpdateBatchCertificateRequestValidator
{
    public class WhenCertificateStatusInvalid : UpdateBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            Setup();

            long uln = 1234;
            int standardCode = 1234;
            string certificateReference = "1234";
            string status = "Deleted";

            AddMockCertificate(uln, standardCode, certificateReference, status);

            CreateBatchCertificateRequest request = Builder<CreateBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = uln)
                .With(i => i.StandardCode = standardCode)
                .With(i => i.UkPrn = 10000000)
                .With(i => i.CertificateReference = certificateReference)
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                                .With(cd => cd.ContactPostCode = "AA11AA")
                                .With(cd => cd.AchievementDate = DateTime.UtcNow)
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
