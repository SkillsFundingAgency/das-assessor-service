using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    [TestFixture(typeof(Certificate))]
    [TestFixture(typeof(FrameworkCertificate))]
    public class WhenSystemRequestsCertificateReprint<T>
        : CertificatePrintRequestTestBase<T>
        where T : CertificateBase, new()
    {
        [Test]
        public async Task ThenPrintStateIsUpdatedAndCertificateReturned()
        {
            // Arrange
            var originalCreatedAt = _certificate.CreatedAt;

            // Act
            var result = await _sut.UpdateReprintRequested<T>(
                _certificate.Id,
                DateTimeUtcNow,
                RequestedBy);

            // Assert
            AssertPrintState(
                result,
                CertificateStatus.Reprint);

            result.CreatedAt.Should().Be(originalCreatedAt);
        }

        [Test]
        public async Task ThenExistingAddressAndAssessmentDataArePreserved()
        {
            // Arrange
            var originalData = Clone(_certificate.CertificateData);
            var originalReference = _certificate.CertificateReference;
            var originalDateOfBirth = _certificate.DateOfBirth;
            var originalProviderUkPrn = _certificate.ProviderUkPrn;

            // Act
            var result = await _sut.UpdateReprintRequested<T>(
                _certificate.Id,
                DateTimeUtcNow,
                RequestedBy);

            // Assert
            result.CertificateData.Should().BeEquivalentTo(originalData);
            result.CertificateReference.Should().Be(originalReference);
            result.DateOfBirth.Should().Be(originalDateOfBirth);
            result.ProviderUkPrn.Should().Be(originalProviderUkPrn);

            VerifySavedOnce();
        }

        [Test]
        public async Task ThenAnIndependentAuditSnapshotIsRecorded()
        {
            // Act
            var result = await _sut.UpdateReprintRequested<T>(
                _certificate.Id,
                DateTimeUtcNow,
                RequestedBy);

            // Assert
            AssertIndependentAuditSnapshot(
                result,
                CertificateStatus.Reprint,
                CertificateActions.Reprint);
        }

        [Test]
        public async Task AndCertificateDoesNotExistThenNothingIsSavedOrLogged()
        {
            // Arrange
            Func<Task> act = () => _sut.UpdateReprintRequested<T>(
                Guid.NewGuid(),
                DateTimeUtcNow,
                RequestedBy);

            // Act / Assert
            await act.Should().ThrowAsync<NotFoundException>();

            VerifyNothingSavedOrLogged();
        }

        [Test]
        public async Task AndIdentifierBelongsToAnotherTypeThenItIsNotUpdated()
        {
            // Arrange
            var originalStatus = _otherCertificate.Status;
            var originalRequestedAt = _otherCertificate.PrintRequestedAt;
            var originalRequestedBy = _otherCertificate.PrintRequestedBy;
            var originalBatchNumber = _otherCertificate.BatchNumber;
            var originalData = Clone(_otherCertificate.CertificateData);

            Func<Task> act = () => _sut.UpdateReprintRequested<T>(
                _otherCertificate.Id,
                DateTimeUtcNow,
                RequestedBy);

            // Act / Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _otherCertificate.Status.Should().Be(originalStatus);
            _otherCertificate.PrintRequestedAt.Should()
                .Be(originalRequestedAt);
            _otherCertificate.PrintRequestedBy.Should()
                .Be(originalRequestedBy);
            _otherCertificate.BatchNumber.Should()
                .Be(originalBatchNumber);
            _otherCertificate.CertificateData.Should()
                .BeEquivalentTo(originalData);

            VerifyNothingSavedOrLogged();
        }
    }
}