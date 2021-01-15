using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_certificates_updated : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        private static DateTime _statusAt = DateTime.UtcNow;
       
        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_printed()
        {
            // Arrange
            var certificatePrintStatusUpdatesPrintedRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch111,
                CertificateReference = _certificateReferenceReprintedAfterPrinted,
                Status = CertificateStatus.Printed,
                StatusAt = _statusAt
            };

            // Act
            _response = await _sut.Handle(certificatePrintStatusUpdatesPrintedRequest,
                new CancellationToken());

            // Assert

            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_printed()
        {
            // Arrange
            var certificatePrintStatusUpdatesPrintedRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch111,
                CertificateReference = _certificateReferenceReprintedAfterPrinted,
                Status = CertificateStatus.Printed,
                StatusAt = _statusAt
            };

            // Act
            _response = await _sut.Handle(certificatePrintStatusUpdatesPrintedRequest,
                new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReferenceReprintedAfterPrinted), _batch111, CertificateStatus.Printed, _statusAt, null, false),
                Times.Once());
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_not_delivered()
        {
            // Arrange
            var certificatePrintStatusUpdatesNotDeliveredRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch222,
                CertificateReference = _certificateReferenceDeletedAfterPrinted,
                Status = CertificateStatus.NotDelivered,
                ReasonForChange = string.Empty,
                StatusAt = _statusAt
            };

            // Act
            _response = await _sut.Handle(certificatePrintStatusUpdatesNotDeliveredRequest,
                new CancellationToken());

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_not_delivered()
        {
            // Arrange
            var certificatePrintStatusUpdatesNotDeliveredRequest = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _batch222,
                CertificateReference = _certificateReferenceDeletedAfterPrinted,
                Status = CertificateStatus.NotDelivered,
                ReasonForChange = string.Empty,
                StatusAt = _statusAt
            };

            // Act
            _response = await _sut.Handle(certificatePrintStatusUpdatesNotDeliveredRequest,
                new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReferenceDeletedAfterPrinted), _batch222, CertificateStatus.NotDelivered, _statusAt, string.Empty, false),
                Times.Once());
        }
    }
}
