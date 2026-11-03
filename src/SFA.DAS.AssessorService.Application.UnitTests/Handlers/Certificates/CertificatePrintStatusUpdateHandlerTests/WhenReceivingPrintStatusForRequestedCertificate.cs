using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.CertificatesPrintStatusUpdateHandlerTests
{
    [TestFixture(typeof(Certificate))]
    [TestFixture(typeof(FrameworkCertificate))]
    public class WhenReceivingPrintStatusForRequestedCertificate<T>
        where T : CertificateBase, new()
    {
        private const int CurrentBatch = 42;
        private const int PreviousBatch = 41;
        private const string Reference = "00123456";

        private static DateTime RequestedAt =>
            new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc);

        private static DateTime SentAt =>
            new DateTime(2026, 11, 2, 10, 0, 0, DateTimeKind.Utc);

        private static DateTime NotificationAt =>
            new DateTime(2026, 11, 3, 11, 0, 0, DateTimeKind.Utc);

        private Mock<ICertificateRepository> _repository;
        private Mock<IBatchLogQueryRepository> _batchRepository;
        private Mock<IMediator> _mediator;
        private CertificatePrintStatusUpdateHandler _sut;

        private T _certificate;
        private CertificateBatchLog _currentBatch;
        private CertificateBatchLog _previousBatch;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ICertificateRepository>();
            _batchRepository = new Mock<IBatchLogQueryRepository>();
            _mediator = new Mock<IMediator>();

            _currentBatch = new CertificateBatchLog
            {
                BatchNumber = CurrentBatch,
                CertificateReference = Reference,
                Status = CertificateStatus.SentToPrinter,
                StatusAt = SentAt
            };

            _previousBatch = new CertificateBatchLog
            {
                BatchNumber = PreviousBatch,
                CertificateReference = Reference,
                Status = CertificateStatus.Printed,
                StatusAt = SentAt.AddDays(-10)
            };

            _certificate = new T
            {
                Id = Guid.NewGuid(),
                CertificateReference = Reference,
                Status = CertificateStatus.SentToPrinter,
                BatchNumber = CurrentBatch,
                CertificateBatchLog = _currentBatch,
                PrintRequestedAt = RequestedAt,
                PrintRequestedBy = "Requesting user",
                CreatedAt = RequestedAt.AddMonths(-1),

                // Deliberately later than the notification event:
                // current print processing must use the batch event date.
                UpdatedAt = NotificationAt.AddDays(1)
            };

            _repository
                .Setup(x => x.GetCertificate<Certificate>(Reference))
                .ReturnsAsync(() => _certificate as Certificate);

            _repository
                .Setup(x => x.GetCertificate<FrameworkCertificate>(Reference))
                .ReturnsAsync(() => _certificate as FrameworkCertificate);

            _repository
                .Setup(x => x.UpdatePrintStatus(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            _batchRepository
                .Setup(x => x.GetCertificateBatchLog(
                    CurrentBatch, Reference))
                .ReturnsAsync(_currentBatch);

            _batchRepository
                .Setup(x => x.GetCertificateBatchLog(
                    PreviousBatch, Reference))
                .ReturnsAsync(_previousBatch);

            AllowBatch(CurrentBatch);
            AllowBatch(PreviousBatch);

            _sut = new CertificatePrintStatusUpdateHandler(
                _repository.Object,
                _batchRepository.Object,
                _mediator.Object,
                Mock.Of<ILogger<CertificatePrintStatusUpdateHandler>>());
        }

        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public async Task ThenCurrentBatchNotificationUpdatesCertificateAndBatch(
            string status)
        {
            // Arrange
            var request = CreateRequest(
                CurrentBatch, status, NotificationAt);

            // Act
            var result = await _sut.Handle(
                request, CancellationToken.None);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();

            VerifyUpdate(request, true, true);

            _certificate.PrintRequestedAt.Should().Be(RequestedAt);
        }

        [Test]
        public async Task AndNotificationIsForAnOlderBatchThenOnlyThatBatchIsUpdated()
        {
            // Arrange
            var request = CreateRequest(
                PreviousBatch,
                CertificateStatus.Delivered,
                _previousBatch.StatusAt.AddHours(1));

            // Act
            var result = await _sut.Handle(
                request, CancellationToken.None);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();

            VerifyUpdate(request, false, true);

            _certificate.PrintStatus.Should()
                .Be(CertificateStatus.SentToPrinter);
            _certificate.PrintStatusAt.Should().Be(SentAt);
            _certificate.PrintRequestedAt.Should().Be(RequestedAt);
        }

        [TestCase(CertificateStatus.Reprint)]
        [TestCase(CertificateStatus.Delivered)]
        public async Task AndBatchHasBeenClearedThenNotificationCannotRestoreCurrentPrintStatus(
            string certificateStatus)
        {
            // Arrange
            var latestRequestAt = NotificationAt.AddHours(1);

            _certificate.Status = certificateStatus;
            _certificate.BatchNumber = null;
            _certificate.PrintRequestedAt = latestRequestAt;

            var request = CreateRequest(
                CurrentBatch,
                CertificateStatus.Delivered,
                NotificationAt);

            // Act
            var result = await _sut.Handle(
                request, CancellationToken.None);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();

            VerifyUpdate(request, false, true);

            _certificate.PrintStatus.Should().BeNull();
            _certificate.PrintStatusAt.Should().BeNull();
            _certificate.PrintRequestedAt.Should().Be(latestRequestAt);
        }

        private void AllowBatch(int batchNumber)
        {
            _mediator
                .Setup(x => x.Send(
                    It.Is<GetBatchLogRequest>(
                        r => r.BatchNumber == batchNumber),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BatchLogResponse
                {
                    BatchNumber = batchNumber
                });
        }

        private static CertificatePrintStatusUpdateRequest CreateRequest(
            int batchNumber,
            string status,
            DateTime statusAt)
        {
            return new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = batchNumber,
                CertificateReference = Reference,
                Status = status,
                StatusAt = statusAt,
                ReasonForChange = "Printer notification"
            };
        }

        private void VerifyUpdate(
            CertificatePrintStatusUpdateRequest request,
            bool updateCertificate,
            bool updateBatch)
        {
            _repository.Verify(
                x => x.UpdatePrintStatus(
                    _certificate.Id,
                    request.BatchNumber,
                    request.Status,
                    request.StatusAt,
                    request.ReasonForChange,
                    updateCertificate,
                    updateBatch),
                Times.Once);
        }
    }
}