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
       
        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();

            var certificatePrintStatusUpdates = new List<CertificatePrintStatusUpdate>
            {
                new CertificatePrintStatusUpdate
                {
                    BatchNumber = _batch111,
                    CertificateReference = _certificateReference1,
                    Status = CertificateStatus.Printed,
                    StatusAt = _statusAt
                },
                new CertificatePrintStatusUpdate
                {
                    BatchNumber = _batch222,
                    CertificateReference = _certificateReference2,
                    Status = CertificateStatus.Delivered,
                    ReasonForChange = string.Empty,
                    StatusAt = _statusAt
                },
                new CertificatePrintStatusUpdate
                {
                    BatchNumber = _batch222,
                    CertificateReference = _certificateReference3,
                    Status = CertificateStatus.NotDelivered,
                    ReasonForChange = _certificateNotDeliveredReason1,
                    StatusAt = _statusAt
                }
            };

            _response = await _sut.Handle(
                new CertificatesPrintStatusUpdateRequest
                {
                    CertificatePrintStatusUpdates = certificatePrintStatusUpdates
                }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_true()
        {
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public void Then_repository_update_print_status_is_called()
        {
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference1), _batch111, CertificateStatus.Printed, _statusAt, null, true),
                Times.Once());

            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference2), _batch222, CertificateStatus.Delivered, _statusAt, string.Empty, true),
                Times.Once());

            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference3), _batch222, CertificateStatus.NotDelivered, _statusAt, _certificateNotDeliveredReason1, true),
                Times.Once());
        }
    }
}
