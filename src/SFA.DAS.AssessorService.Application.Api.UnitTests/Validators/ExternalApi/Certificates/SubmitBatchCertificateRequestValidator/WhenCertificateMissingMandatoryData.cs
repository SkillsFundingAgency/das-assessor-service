using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.SubmitBatchCertificateRequestValidator
{
    public class WhenCertificateMissingMandatoryData
        : SubmitBatchCertificateRequestValidatorTestBase
    {
        [Test]
        public async Task WhenAchievementDateIsMissing_ThenValidationFails()
        {
            // Arrange
            var request = new SubmitBatchCertificateRequest
            {
                Uln = 1234567890,
                StandardCode = 1,
                UkPrn = 12345678,
                FamilyName = "Test",
                CertificateReference = "1234567890-1"
            };

            var certificate = new Certificate
            {
                CertificateReference = request.CertificateReference,
                OrganisationId = new Guid("12345678123456781234567812345678"),
                Status = CertificateStatus.Draft,
                CertificateData = new CertificateData
                {
                    LearnerGivenNames = "Alex",
                    LearnerFamilyName = request.FamilyName,
                    Version = "1.0",
                    OverallGrade = CertificateGrade.Pass,
                    AchievementDate = null
                }
            };

            CertificateRepositoryMock
                .Setup(x => x.GetCertificate(request.Uln, request.StandardCode))
                .ReturnsAsync(certificate);

            // Act
            var result = await Validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(error =>
                error.PropertyName ==
                    nameof(SubmitBatchCertificateRequest.CertificateReference) &&
                error.ErrorMessage == "Certificate is missing mandatory data");
        }
    }
}