using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
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
        public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull_IncludeLogsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, false))
                .ReturnsAsync((Certificate?)null);

            var request = new GetCertificateRequest(id, includeLogs: false);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, false), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsSameCertificateInstance_WhenRepositoryReturnsCertificate_IncludeLogsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cert = new Certificate { Id = id };

            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, false))
                .ReturnsAsync(cert);

            var request = new GetCertificateRequest(id, includeLogs: false);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(cert);
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, false), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsSameCertificateInstance_WhenRepositoryReturnsCertificate_IncludeLogsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cert = new Certificate { Id = id };

            _certificateRepositoryMock
                .Setup(r => r.GetCertificate<Certificate>(id, true))
                .ReturnsAsync(cert);

            var request = new GetCertificateRequest(id, includeLogs: true);

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(cert);
            _certificateRepositoryMock.Verify(r => r.GetCertificate<Certificate>(id, true), Times.Once);
        }
    }
}
