using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    [TestFixture]
    public class WhenSystemRequestsStandardCertificatePrint
        : CertificatePrintRequestTestBase<Certificate>
    {
        [SetUp]
        public void PrepareFirstPrint()
        {
            _certificate.Status = CertificateStatus.Submitted;
            _certificate.PrintRequestedAt = null;
            _certificate.PrintRequestedBy = null;
        }

        [Test]
        public async Task ThenPrintStateIsUpdatedAndCertificateReturned()
        {
            // Arrange
            var originalCreatedAt = _certificate.CreatedAt;
            var beforeUpdatedAt = DateTime.UtcNow;

            // Act
            var result = await _sut.UpdatePrintRequested<Certificate>(
                _certificate.Id,
                _address,
                RequestedAt,
                RequestedBy);

            var afterUpdatedAt = DateTime.UtcNow;

            // Assert
            AssertPrintState(
                result,
                CertificateStatus.PrintRequested,
                beforeUpdatedAt,
                afterUpdatedAt);

            result.CreatedAt.Should().Be(originalCreatedAt);
        }

        [Test]
        public async Task ThenAddressIsUpdatedAndAssessmentDataIsPreserved()
        {
            // Arrange
            var expectedData = Clone(_certificate.CertificateData);
            var originalReference = _certificate.CertificateReference;
            var originalDateOfBirth = _certificate.DateOfBirth;
            var originalProviderUkPrn = _certificate.ProviderUkPrn;

            expectedData.ContactName = _address.ContactName;
            expectedData.ContactOrganisation = _address.ContactOrganisation;
            expectedData.ContactAddLine1 = _address.ContactAddLine1;
            expectedData.ContactAddLine2 = _address.ContactAddLine2;
            expectedData.ContactAddLine3 = _address.ContactAddLine3;
            expectedData.ContactAddLine4 = _address.ContactAddLine4;
            expectedData.ContactPostCode = _address.ContactPostCode;

            // Act
            var result = await _sut.UpdatePrintRequested<Certificate>(
                _certificate.Id,
                _address,
                RequestedAt,
                RequestedBy);

            // Assert
            result.CertificateData.Should().BeEquivalentTo(expectedData);
            result.CertificateReference.Should().Be(originalReference);
            result.DateOfBirth.Should().Be(originalDateOfBirth);
            result.ProviderUkPrn.Should().Be(originalProviderUkPrn);

            VerifySavedOnce();
        }

        [Test]
        public async Task ThenAnIndependentAuditSnapshotIsRecorded()
        {
            // Act
            var result = await _sut.UpdatePrintRequested<Certificate>(
                _certificate.Id,
                _address,
                RequestedAt,
                RequestedBy);

            // Assert
            AssertIndependentAuditSnapshot(
                result,
                CertificateStatus.PrintRequested);
        }

        [Test]
        public async Task AndCertificateDoesNotExistThenNothingIsSavedOrLogged()
        {
            // Arrange
            Func<Task> act = () =>
                _sut.UpdatePrintRequested<Certificate>(
                    Guid.NewGuid(),
                    _address,
                    RequestedAt,
                    RequestedBy);

            // Act / Assert
            await act.Should().ThrowAsync<NotFoundException>();

            VerifyNothingSavedOrLogged();
        }

        [Test]
        public async Task AndIdentifierBelongsToFrameworkThenItIsNotUpdated()
        {
            // Arrange
            var originalStatus = _otherCertificate.Status;
            var originalRequestedAt = _otherCertificate.PrintRequestedAt;
            var originalRequestedBy = _otherCertificate.PrintRequestedBy;
            var originalBatchNumber = _otherCertificate.BatchNumber;
            var originalData = Clone(_otherCertificate.CertificateData);

            Func<Task> act = () =>
                _sut.UpdatePrintRequested<Certificate>(
                    _otherCertificate.Id,
                    _address,
                    RequestedAt,
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