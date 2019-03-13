using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Certificates.BatchCertificateRequestValidator
{
    public class WhenAchievementDateInFuture : BatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            BatchCertificateRequest request = Builder<BatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 99)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .With(i => i.CertificateReference = null)
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                                .With(cd => cd.ContactPostCode = "AA11AA")
                                .With(cd => cd.AchievementDate = DateTime.UtcNow.AddHours(1))
                                .With(cd => cd.OverallGrade = "Pass")
                                .With(cd => cd.CourseOption = "English")
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