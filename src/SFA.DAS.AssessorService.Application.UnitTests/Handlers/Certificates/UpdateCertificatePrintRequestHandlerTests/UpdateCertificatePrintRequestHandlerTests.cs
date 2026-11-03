using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Mono.Cecil.Cil;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatePrintRequestHandlerTests
{
    [TestFixture]
    public class UpdateCertificatePrintRequestHandlerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ICertificateRepository> _certificateRepository;
        private UpdateCertificatePrintRequestHandler _sut;

        private Certificate _certificate;
        private UpdateCertificatePrintRequestCommand _request;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _certificateRepository = new Mock<ICertificateRepository>();

            _certificate = new Certificate
            {
                Id = Guid.NewGuid(),
                Status = CertificateStatus.Submitted,
                PrintRequestedAt = null,
                CertificateData = new CertificateData
                {
                    EpaDetails = new EpaDetails
                    {
                        LatestEpaOutcome = EpaOutcome.Pass
                    }
                }
            };

            SetLatestEpaOutcome(EpaOutcome.Pass);

            _request = new UpdateCertificatePrintRequestCommand
            {
                CertificateId = _certificate.Id,
                PrintRequestedAt = new DateTime(2026, 11, 3, 12, 0, 0, DateTimeKind.Utc),
                PrintRequestedBy = "Apprentice",
                Address = new CertificatePrintAddress
                {
                    ContactName = "Delivery recipient",
                    ContactOrganisation = "Organisation",
                    ContactAddLine1 = "10 Example Road",
                    ContactAddLine2 = "Example Village",
                    ContactAddLine3 = "Example Town",
                    ContactAddLine4 = "Example County",
                    ContactPostCode = "AB1 2CD"
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.Is<GetCertificateRequest>(r =>
                        r.CertificateId == _request.CertificateId &&
                        !r.IncludeLogs),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_certificate);

            _certificateRepository
                .Setup(x => x.UpdatePrintRequested<Certificate>(
                    It.IsAny<Guid>(),
                    It.IsAny<CertificatePrintAddress>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()))
                .ReturnsAsync(_certificate);

            _sut = new UpdateCertificatePrintRequestHandler(
                _mediator.Object,
                _certificateRepository.Object,
                Mock.Of<ILogger<UpdateCertificatePrintRequestHandler>>());
        }

        public async Task WhenCertificateIsEligible_ThenPrintRequestIsPassedToRepository()
        {
            // Arrange
            SetLatestEpaOutcome(EpaOutcome.Pass);
            _certificate.Status = CertificateStatus.Submitted;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);

            _mediator.Verify(x => x.Send(
                It.Is<GetCertificateRequest>(r =>
                    r.CertificateId == _request.CertificateId &&
                    !r.IncludeLogs),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _certificateRepository.Verify(
                x => x.UpdatePrintRequested<Certificate>(
                    _request.CertificateId,
                    _request.Address,
                    _request.PrintRequestedAt,
                    _request.PrintRequestedBy),
                Times.Once);

            _certificateRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WhenCertificateIsNotFound_ThenThrowsWithoutRequestingPrint()
        {
            // Arrange
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetCertificateRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Certificate)null);

            Func<Task> act = () => _sut.Handle(
                _request,
                CancellationToken.None);

            // Act / Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _certificateRepository.VerifyNoOtherCalls();
        }

        [TestCase(EpaOutcome.Fail, CertificateStatus.Submitted)]
        [TestCase(null, CertificateStatus.Submitted)]
        [TestCase("", CertificateStatus.Submitted)]
        [TestCase(EpaOutcome.Pass, null)]
        [TestCase(EpaOutcome.Pass, "")]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Draft)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.PrintRequested)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Reprint)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.SentToPrinter)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Printed)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Delivered)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.NotDelivered)]
        [TestCase(EpaOutcome.Pass, CertificateStatus.Deleted)]
        public async Task WhenCertificateIsIneligible_ThenThrowsWithoutRequestingPrint(
            string outcome,
            string status)
        {
            // Arrange
            SetLatestEpaOutcome(outcome);
            _certificate.Status = status;

            Func<Task> act = () => _sut.Handle(
                _request,
                CancellationToken.None);

            // Act / Assert
            await act.Should().ThrowAsync<ArgumentException>();

            _certificateRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WhenPrintWasAlreadyRequested_ThenThrowsWithoutRequestingPrint()
        {
            // Arrange
            _certificate.PrintRequestedAt =
                _request.PrintRequestedAt.AddDays(-1);

            Func<Task> act = () => _sut.Handle(
                _request,
                CancellationToken.None);

            // Act / Assert
            await act.Should().ThrowAsync<ArgumentException>();

            _certificateRepository.VerifyNoOtherCalls();
        }

        private void SetLatestEpaOutcome(string outcome)
        {
            _certificate.CertificateData.EpaDetails.LatestEpaOutcome = outcome;

            typeof(CertificateBase)
                .GetProperty(nameof(CertificateBase.LatestEPAOutcome))
                .SetValue(_certificate, outcome);
        }
    }
}