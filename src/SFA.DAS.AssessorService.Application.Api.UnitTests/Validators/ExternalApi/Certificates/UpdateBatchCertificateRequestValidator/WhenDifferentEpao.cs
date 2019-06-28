using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.UpdateBatchCertificateRequestValidator
{
    public class WhenDifferentEpao : UpdateBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            UpdateBatchCertificateRequest request = Builder<UpdateBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 1)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 99999999)
                .With(i => i.FamilyName = "Test")
                .With(i => i.CertificateReference = "1234567890-1")
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                    .With(cd => cd.ContactPostCode = "AA11AA")
                    .With(cd => cd.AchievementDate = DateTime.UtcNow)
                    .With(cd => cd.OverallGrade = "Pass")
                    .With(cd => cd.CourseOption = null)
                    .Build())
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
        }
    }
}
