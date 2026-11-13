using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Helpers
{
    [TestFixture]
    public class WhenGettingCertificateStatusDate
    {
        private readonly DateTime _requestedAt = new(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _statusAt = new(2026, 10, 8, 10, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _updatedAt = new(2026, 10, 9, 11, 0, 0, DateTimeKind.Utc);

        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Reprint)]
        public void ThenQueuedRequestsUseTheRequestDate(string status)
        {
            // Arrange
            var requestedAt = _requestedAt;

            // Act
            var result = CertificateStatus.GetStatusDate(status, requestedAt, _statusAt, _updatedAt);

            // Assert
            result.Should().Be(requestedAt);
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public void ThenPrinterStatusesUseTheEventDate(string status)
        {
            // Arrange
            var eventDate = _statusAt;

            // Act
            var result = CertificateStatus.GetStatusDate(status, _requestedAt, eventDate, _updatedAt);

            // Assert
            result.Should().Be(eventDate);
        }

        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Reprint)]
        public void AndRequestDateIsMissingThenNoOtherDateIsSubstituted(string status)
        {
            // Arrange
            DateTime? requestedAt = null;

            // Act
            var result = CertificateStatus.GetStatusDate(status, requestedAt, _statusAt, _updatedAt);

            // Assert
            result.Should().BeNull();
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public void AndCurrentBatchDateIsMissingThenNoOtherDateIsSubstituted(string status)
        {
            // Arrange
            DateTime? statusAt = null;

            // Act
            var result = CertificateStatus.GetStatusDate(status, _requestedAt, statusAt, _updatedAt);

            // Assert
            result.Should().BeNull();
        }

        [TestCase(CertificateStatus.Draft)]
        [TestCase(CertificateStatus.Submitted)]
        [TestCase(CertificateStatus.ToBeApproved)]
        [TestCase(CertificateStatus.SentForApproval)]
        public void ThenAssessmentStatusesRetainTheirExistingDate(string status)
        {
            // Arrange
            var updatedAt = _updatedAt;

            // Act
            var result = CertificateStatus.GetStatusDate(status, _requestedAt, _statusAt, updatedAt);

            // Assert
            result.Should().Be(updatedAt);
        }

        [Test]
        public void ThenPrintRequestedHasReadableLabels()
        {
            // Arrange
            var status = CertificateStatus.PrintRequested;

            // Act
            var label = CertificateStatus.GetDisplayName(status);
            var dateLabel = CertificateStatus.GetStatusDateDisplayName(status);

            // Assert
            label.Should().Be("Print requested");
            dateLabel.Should().Be("Date print requested");
        }
    }
}
