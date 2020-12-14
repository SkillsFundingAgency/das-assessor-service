using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_status_not_print_status : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        
        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();
        }

        [TestCase("Approved")]
        [TestCase("Cancelled")]
        [TestCase("Deleted")]
        [TestCase("Draft")]
        [TestCase("NoCertificate")]
        [TestCase("Rejected")]
        [TestCase("Reprint")]
        [TestCase("SentForApproval")]
        [TestCase("ToBeApproved")]
        public async Task Then_validation_response_is_valid_false(string invalidPrintStatus)
        {
            // Arrange
            var invalidCertificatePrintStatusUpdateRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch111,
                CertificateReference = _certificateReference1,
                Status = invalidPrintStatus,
                StatusAt = DateTime.UtcNow
            };
            
            // Act
            _response = await _sut.Handle(invalidCertificatePrintStatusUpdateRequest, 
                new CancellationToken());

            // Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);

            _response.Errors[0].Field.Should().Be("CertificatePrintStatuses");
            _response.Errors.Should().Contain(p => p.ErrorMessage.Contains(invalidPrintStatus));
            
        }

        [TestCase("Approved")]
        [TestCase("Cancelled")]
        [TestCase("Deleted")]
        [TestCase("Draft")]
        [TestCase("NoCertificate")]
        [TestCase("Rejected")]
        [TestCase("Reprint")]
        [TestCase("SentForApproval")]
        [TestCase("ToBeApproved")]
        public async Task Then_repository_update_print_status_is_not_called(string invalidPrintStatus)
        {
            // Arrange
            var invalidCertificatePrintStatusUpdateRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch111,
                CertificateReference = _certificateReference1,
                Status = invalidPrintStatus,
                StatusAt = DateTime.UtcNow
            };

            // Act
            _response = await _sut.Handle(invalidCertificatePrintStatusUpdateRequest,
                new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<bool>()), 
                Times.Never());
        }
    }
}
