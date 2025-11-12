using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    public class When_called_to_get_certificates
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private GetCertificatesUlnHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _sut = new GetCertificatesUlnHandler(_certificateRepository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_And_List_Returned()
        {
            // Arrange
            var uln = 1234567890L;
            var expectedList = new List<ApprenticeCertificateSummary>
            {
                new ApprenticeCertificateSummary { CertificateId = Guid.NewGuid(), CertificateType = "Standard", CourseCode = "99" },
                new ApprenticeCertificateSummary { CertificateId = Guid.NewGuid(), CertificateType = "Standard", CourseCode = "100" }
            };

            _certificateRepository
                .Setup(r => r.GetPrintableCertificates(uln))
                .ReturnsAsync(expectedList);

            var request = new GetCertificatesUlnRequest { Uln = uln };

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Certificates.Should().BeEquivalentTo(expectedList);
            _certificateRepository.Verify(r => r.GetPrintableCertificates(uln), Times.Once);
        }

        [Test]
        public async Task Then_Empty_List_Is_Returned_When_No_Certificates_Found()
        {
            // Arrange
            var uln = 1234567890L;

            _certificateRepository
                .Setup(r => r.GetPrintableCertificates(uln))
                .ReturnsAsync(new List<ApprenticeCertificateSummary>());

            var request = new GetCertificatesUlnRequest { Uln = uln };

            // Act
            var result = await _sut.Handle(request, CancellationToken.None);

            // Assert
            result.Certificates.Should().BeEmpty();
            _certificateRepository.Verify(r => r.GetPrintableCertificates(uln), Times.Once);
        }
    }
}
