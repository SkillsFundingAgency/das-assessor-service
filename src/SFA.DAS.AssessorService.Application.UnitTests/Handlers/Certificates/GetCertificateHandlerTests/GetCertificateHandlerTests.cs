using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Handlers.Certificates;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.GetCertificateHandlerTests
{
    [TestFixture]
    public class GetCertificateHandlerTests
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private GetCertificateHandler _sut;

        [SetUp]
        public void Arrange()
        {
            _certificateRepositoryMock = new Mock<ICertificateRepository>();
            _sut = new GetCertificateHandler(_certificateRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsNull_WhenCertificateNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, It.IsAny<bool>()))
                .ReturnsAsync((Certificate)null);

            var request = new GetCertificateRequest(id, includeLogs: false);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, false), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsCertificate_WhenPrintRequestedAlreadySet()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingPrintRequestedAt = DateTime.UtcNow.AddDays(-2);
            var existingPrintRequestedBy = "existing.user";

            var cert = new Certificate
            {
                Id = id,
                PrintRequestedAt = existingPrintRequestedAt,
                PrintRequestedBy = existingPrintRequestedBy,
                CertificateLogs = new List<CertificateLog>
                {
                    new CertificateLog { Action = CertificateActions.PrintRequest, EventTime = DateTime.UtcNow.AddDays(-1), Username = "user" }
                }
            };

            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, true))
                .ReturnsAsync(cert);

            var request = new GetCertificateRequest(id, includeLogs: true);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.PrintRequestedAt.Should().Be(existingPrintRequestedAt);
            result.PrintRequestedBy.Should().Be(existingPrintRequestedBy);
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, true), Times.Once);
        }

        [Test]
        public async Task Handle_PopulatesPrintRequestedFromLatestLog_WhenMissing()
        {
            // Arrange
            var id = Guid.NewGuid();
            var older = DateTime.UtcNow.AddDays(-3);
            var newer = DateTime.UtcNow.AddDays(-1);

            var cert = new Certificate
            {
                Id = id,
                PrintRequestedAt = null,
                PrintRequestedBy = null,
                CertificateLogs = new List<CertificateLog>
                {
                    new CertificateLog { Action = CertificateActions.PrintRequest, EventTime = older, Username = "older.user" },
                    new CertificateLog { Action = CertificateActions.PrintRequest, EventTime = newer, Username = "newer.user" },
                    new CertificateLog { Action = CertificateActions.Submit, EventTime = DateTime.UtcNow.AddDays(-5), Username = "submit.user" }
                }
            };

            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, true))
                .ReturnsAsync(cert);

            var request = new GetCertificateRequest(id, includeLogs: true);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.PrintRequestedAt.Should().Be(newer);
            result.PrintRequestedBy.Should().Be("newer.user");
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, true), Times.Once);
        }

        [Test]
        public async Task Handle_LeavesPrintRequestedNull_WhenNoPrintRequestLog()
        {
            // Arrange
            var id = Guid.NewGuid();

            var cert = new Certificate
            {
                Id = id,
                PrintRequestedAt = null,
                PrintRequestedBy = null,
                CertificateLogs = new List<CertificateLog>
                {
                    new CertificateLog { Action = CertificateActions.Submit, EventTime = DateTime.UtcNow.AddDays(-5), Username = "submit.user" }
                }
            };

            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, true))
                .ReturnsAsync(cert);

            var request = new GetCertificateRequest(id, includeLogs: true);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.PrintRequestedAt.Should().BeNull();
            result.PrintRequestedBy.Should().BeNull();
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, true), Times.Once);
        }
    }
}
