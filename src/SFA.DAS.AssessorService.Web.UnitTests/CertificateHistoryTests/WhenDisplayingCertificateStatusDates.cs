using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateHistoryTests
{
    [TestFixture]
    public class WhenDisplayingCertificateStatusDates
    {
        private CertificateHistoryViewModel _sut;
        private CertificateSummaryResponse _certificate;

        [SetUp]
        public void SetUp()
        {
            _sut = new CertificateHistoryViewModel();
            _certificate = new CertificateSummaryResponse
            {
                PrintRequestedAt = new DateTime(2026, 10, 1),
                PrintStatusAt = new DateTime(2026, 10, 8),
                LatestStatusDatetime = new DateTime(2026, 10, 9)
            };
        }

        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Reprint)]
        public void ThenQueuedRequestsShowTheRequestDate(string status)
        {
            // Arrange
            _certificate.Status = status;

            // Act
            var result = _sut.GetCertificateStatusDate(_certificate);

            // Assert
            result.Should().Be(_certificate.PrintRequestedAt);
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public void ThenPrinterStatusesShowTheEventDate(string status)
        {
            // Arrange
            _certificate.Status = status;

            // Act
            var result = _sut.GetCertificateStatusDate(_certificate);

            // Assert
            result.Should().Be(_certificate.PrintStatusAt);
        }

        [Test]
        public void AndCurrentBatchWasClearedThenNoPrintingDateIsShown()
        {
            // Arrange
            _certificate.Status = CertificateStatus.Delivered;
            _certificate.PrintStatusAt = null;

            // Act
            var result = _sut.GetCertificateStatusDate(_certificate);

            // Assert
            result.Should().BeNull();
        }
    }
}
