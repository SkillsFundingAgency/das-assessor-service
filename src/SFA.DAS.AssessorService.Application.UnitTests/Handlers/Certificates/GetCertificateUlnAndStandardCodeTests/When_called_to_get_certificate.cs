using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    public class When_called_to_get_certificate
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private GetCertificateUlnAndStandardCodeHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _sut = new GetCertificateUlnAndStandardCodeHandler(_certificateRepository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_And_Certificate_Returned()
        {
            // Arrange
            var uln = 1234567890L;
            var standardCode = 123;
            var expectedCertificate = new Certificate { Id = Guid.NewGuid(), Uln = uln, StandardCode = standardCode };

            _certificateRepository
                .Setup(r => r.GetCertificate(uln, standardCode))
                .ReturnsAsync(expectedCertificate);

            var request = new GetCertificateUlnAndStandardCodeRequest
            {
                Uln = uln,
                StandardCode = standardCode
            };

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedCertificate);
            _certificateRepository.Verify(r => r.GetCertificate(uln, standardCode), Times.Once);
        }

        [Test]
        public async Task Then_Null_Is_Returned_If_No_Certificate_Found()
        {
            // Arrange
            var uln = 1234567890L;
            var standardCode = 123;

            _certificateRepository
                .Setup(r => r.GetCertificate(uln, standardCode))
                .ReturnsAsync((Certificate)null);

            var request = new GetCertificateUlnAndStandardCodeRequest
            {
                Uln = uln,
                StandardCode = standardCode
            };

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _certificateRepository.Verify(r => r.GetCertificate(uln, standardCode), Times.Once);
        }
    }
}
