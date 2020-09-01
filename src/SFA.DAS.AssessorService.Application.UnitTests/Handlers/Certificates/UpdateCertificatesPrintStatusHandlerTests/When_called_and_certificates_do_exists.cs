using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class When_called_and_certificates_do_exists : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        private static DateTime _statusChangedAt = DateTime.UtcNow;
       
        [SetUp]
        public async Task Arrange()
        {
            base.BaseArrange();

            var certificatePrintStatuses = new List<CertificatePrintStatus>
            {
                new CertificatePrintStatus
                {
                    BatchNumber = _batchNumber,
                    CertificateReference = _certificateReference1,
                    Status = CertificateStatus.Printed,
                    StatusChangedAt = _statusChangedAt
                },
                new CertificatePrintStatus
                {
                    BatchNumber = _batchNumber,
                    CertificateReference = _certificateReference2,
                    Status = CertificateStatus.Delivered,
                    ReasonForChange = string.Empty,
                    StatusChangedAt = _statusChangedAt
                },
                new CertificatePrintStatus
                {
                    BatchNumber = _batchNumber,
                    CertificateReference = _certificateReference3,
                    Status = CertificateStatus.NotDelivered,
                    ReasonForChange = _certificateNotDeliveredReason1,
                    StatusChangedAt = _statusChangedAt
                }
            };

            _response = await _sut.Handle(
                new UpdateCertificatesPrintStatusRequest
                {
                    CertificatePrintStatuses = certificatePrintStatuses
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
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference1), _batchNumber, CertificateStatus.Printed, _statusChangedAt, null, true),
                Times.Once());

            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference2), _batchNumber, CertificateStatus.Delivered, _statusChangedAt, string.Empty, true),
                Times.Once());

            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference3), _batchNumber, CertificateStatus.NotDelivered, _statusChangedAt, _certificateNotDeliveredReason1, true),
                Times.Once());
        }
    }
}
