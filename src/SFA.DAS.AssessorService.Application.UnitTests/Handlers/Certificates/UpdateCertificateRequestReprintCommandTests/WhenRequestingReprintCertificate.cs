using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Validators;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    [TestFixture]
    public class WhenRequestingReprintCertificate
    {
        private static DateTime RequestedAt =>
            new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);

        private Mock<ICertificateRepository> _repository;
        private Mock<IDateTimeHelper> _dateTimeHelper;
        private ReprintCertificateCommandHandler _sut;

        private Certificate _certificate;
        private ReprintCertificateCommand _request;

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
                BatchNumber = 42,
                CertificateData = new CertificateData
                {
                    SendTo = CertificateSendTo.Apprentice,
                    ContactName = "Test Recipient",
                    ContactAddLine1 = "1 Example Road",
                    ContactPostCode = "CV1 2WT"
                }
            };

            _request = new ReprintCertificateCommand
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

            _sut = new ReprintCertificateCommandHandler(
                _repository.Object,
                _dateTimeHelper.Object,
                new PrintDeliveryAddressValidator());
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
        public async Task AndDeliveryDetailsAreInvalidThenNoReprintIsRequested(
            [Values(
                nameof(CertificateData.SendTo),
                nameof(CertificateData.ContactName),
                nameof(CertificateData.ContactAddLine1),
                nameof(CertificateData.ContactPostCode),
                nameof(CertificateData.ContactOrganisation))]
            string invalidField)
        {
            // Arrange
            var data = _certificate.CertificateData;

            switch (invalidField)
            {
                case nameof(CertificateData.SendTo):
                    data.SendTo = CertificateSendTo.None;
                    break;
                case nameof(CertificateData.ContactName):
                    data.ContactName = null;
                    break;
                case nameof(CertificateData.ContactAddLine1):
                    data.ContactAddLine1 = " ";
                    break;
                case nameof(CertificateData.ContactPostCode):
                    data.ContactPostCode = null;
                    break;
                case nameof(CertificateData.ContactOrganisation):
                    data.SendTo = CertificateSendTo.Employer;
                    data.ContactOrganisation = null;
                    break;
            }

            // Act
            Func<Task> act = () =>
                _sut.Handle(_request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();

            VerifyNoReprintRequested();
        }

        [Test]
        public async Task AndReprintIsAlreadyPendingThenRequestDateIsNotReset()
        {
            // Arrange
            _certificate.Status = CertificateStatus.Reprint;
            _certificate.CertificateData.ContactPostCode = null;
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

            // Act
            Func<Task> act = () =>
                _sut.Handle(_request, CancellationToken.None);

            // Assert
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