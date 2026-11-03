using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Helpers;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    [TestFixture]
    public class WhenRequestingStandardCertificateReprint
    {
        private static DateTime RequestedAt =>
            new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);

        private Mock<ICertificateRepository> _repository;
        private Mock<IDateTimeHelper> _dateTimeHelper;
        private UpdateCertificateRequestReprintCommandHandler _sut;

        private Certificate _certificate;
        private UpdateCertificateRequestReprintCommand _request;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ICertificateRepository>();
            _dateTimeHelper = new Mock<IDateTimeHelper>();

            _certificate = new Certificate
            {
                Id = Guid.NewGuid(),
                Status = CertificateStatus.Delivered,
                PrintRequestedAt = RequestedAt.AddMonths(-1),
                PrintRequestedBy = "Previous user",
                BatchNumber = 42
            };

            _request = new UpdateCertificateRequestReprintCommand
            {
                CertificateId = _certificate.Id,
                Username = "Admin user"
            };

            _repository
                .Setup(x => x.GetCertificate<Certificate>(
                    _request.CertificateId, false))
                .ReturnsAsync(() => _certificate);

            _repository
                .Setup(x => x.UpdateReprintRequested<Certificate>(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => _certificate);

            _dateTimeHelper
                .SetupGet(x => x.DateTimeUtcNow)
                .Returns(RequestedAt);

            _sut = new UpdateCertificateRequestReprintCommandHandler(
                _repository.Object,
                _dateTimeHelper.Object);
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public async Task ThenEligibleReprintUsesTheCurrentUtcDateAndAdminUser(
            string status)
        {
            // Arrange
            _certificate.Status = status;

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(_certificate);

            _repository.Verify(
                x => x.UpdateReprintRequested<Certificate>(
                    _request.CertificateId,
                    RequestedAt,
                    _request.Username),
                Times.Once);

            _dateTimeHelper.VerifyGet(
                x => x.DateTimeUtcNow,
                Times.Once);
        }

        [Test]
        public async Task AndReprintIsAlreadyPendingThenRequestDateIsNotReset()
        {
            // Arrange
            _certificate.Status = CertificateStatus.Reprint;
            var previousDate = _certificate.PrintRequestedAt;
            var previousUser = _certificate.PrintRequestedBy;

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(_certificate);
            result.PrintRequestedAt.Should().Be(previousDate);
            result.PrintRequestedBy.Should().Be(previousUser);

            VerifyNoReprintRequested();
        }

        [TestCase(CertificateStatus.Draft)]
        [TestCase(CertificateStatus.Ready)]
        [TestCase(CertificateStatus.Submitted)]
        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Deleted)]
        [TestCase(CertificateStatus.Cancelled)]
        [TestCase(null)]
        public async Task AndStatusIsIneligibleThenNoReprintIsRequested(
            string status)
        {
            // Arrange
            _certificate.Status = status;
            var previousDate = _certificate.PrintRequestedAt;

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(_certificate);
            result.Status.Should().Be(status);
            result.PrintRequestedAt.Should().Be(previousDate);

            VerifyNoReprintRequested();
        }

        [Test]
        public async Task AndCertificateIsMissingThenNotFoundIsThrown()
        {
            // Arrange
            _certificate = null;

            Func<Task> act = () =>
                _sut.Handle(_request, CancellationToken.None);

            // Act / Assert
            await act.Should().ThrowAsync<NotFoundException>();

            VerifyNoReprintRequested();
        }

        private void VerifyNoReprintRequested()
        {
            _repository.Verify(
                x => x.UpdateReprintRequested<Certificate>(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()),
                Times.Never);

            _dateTimeHelper.VerifyGet(
                x => x.DateTimeUtcNow,
                Times.Never);
        }
    }
}