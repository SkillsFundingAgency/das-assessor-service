using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.UpdateBatchCertificateRequestValidator
{
    public class WhenUpdatedOptionIsDifferentToLearner : UpdateBatchCertificateRequestValidatorTestBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
            UpdateBatchCertificateRequest request = Builder<UpdateBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 5555555556)  // learners course option is "French" which doesn't match the certificate course option
                .With(i => i.StandardCode = 55)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 55555555)
                .With(i => i.FamilyName = "Test")
                .With(i => i.CertificateReference = "5555555556-55")
                .With(i => i.CertificateData = Builder<CertificateData>.CreateNew()
                                .With(cd => cd.ContactPostCode = "AA11AA")
                                .With(cd => cd.AchievementDate = DateTime.UtcNow)
                                .With(cd => cd.OverallGrade = CertificateGrade.Pass)
                                .With(cd => cd.CourseOption = "English")
                                .With(cd => cd.Version = "1.0")
                                .Build())
                .Build();

            _validationResult = await Validator.ValidateAsync(request);
        }

        [Test]
        public void ThenValidationResultShouldBeFalse()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validationResult.Errors.Should().HaveCount(1);
            _validationResult.Errors[0].ErrorMessage.Should().Be("Incorrect course option for learner");
        }
    }
}
