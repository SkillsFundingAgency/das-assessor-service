using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_certificates_do_exists : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        private static DateTime _statusAt = DateTime.UtcNow;

        private CertificatePrintStatusUpdateRequest _certificatePrintStatusUpdateRequestPrinted = new CertificatePrintStatusUpdateRequest
        {
            BatchNumber = _batch111,
            CertificateReference = _certificateReference1,
            Status = CertificateStatus.Printed,
            StatusAt = _statusAt
        };

        private CertificatePrintStatusUpdateRequest _certificatePrintStatusUpdateRequestDelivered = new CertificatePrintStatusUpdateRequest
        {
            BatchNumber = _batch222,
            CertificateReference = _certificateReference2,
            Status = CertificateStatus.Delivered,
            ReasonForChange = string.Empty,
            StatusAt = _statusAt
        };

        private CertificatePrintStatusUpdateRequest _certificatePrintStatusUpdateRequestNotDelivered = new CertificatePrintStatusUpdateRequest
        {
            BatchNumber = _batch222,
            CertificateReference = _certificateReference3,
            Status = CertificateStatus.NotDelivered,
            ReasonForChange = _certificateNotDeliveredReason1,
            StatusAt = _statusAt
        };

        [SetUp]
        public void Arrange()
        {
            base.BaseArrange();
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_printed()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestPrinted, new CancellationToken());

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_printed()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestPrinted, new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference1), _batch111, CertificateStatus.Printed, _statusAt, null, true),
                Times.Once());
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_delivered()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestDelivered, new CancellationToken());

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_delivered()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestDelivered, new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference2), _batch222, CertificateStatus.Delivered, _statusAt, string.Empty, true),
                Times.Once());
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_not_delivered()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestNotDelivered, new CancellationToken());

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_not_delivered()
        {
            // Act
            _response = await _sut.Handle(_certificatePrintStatusUpdateRequestNotDelivered, new CancellationToken());

            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference3), _batch222, CertificateStatus.NotDelivered, _statusAt, _certificateNotDeliveredReason1, true),
                Times.Once());
        }
    }
}
