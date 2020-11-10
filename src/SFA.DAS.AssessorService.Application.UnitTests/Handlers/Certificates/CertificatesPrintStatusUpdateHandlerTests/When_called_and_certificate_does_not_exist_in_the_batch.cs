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
    public class When_called_and_certificate_does_not_exist_in_the_batch : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private ValidationResponse _response;
        private static DateTime _statusChangedAt = _deliveredAt;

        [SetUp]
        public async Task Arrange()
        {
            // Arrange
            base.BaseArrange();
            
            var certificatePrintStatusUpdates = new List<CertificatePrintStatusUpdate>
            {
                new CertificatePrintStatusUpdate
                {
                    BatchNumber = _batch222,
                    CertificateReference = _certificateReference4,
                    Status = CertificateStatus.NotDelivered,
                    StatusAt = _statusChangedAt
                }
            };

            // Act
            _response = await _sut.Handle(
                new CertificatesPrintStatusUpdateRequest
                {
                    CertificatePrintStatusUpdates = certificatePrintStatusUpdates
                }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            // Assert
            _response.IsValid.Should().Be(false);
        }

        [Test]
        public void Then_repository_update_print_status_is_not_called()
        {
            // Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.IsAny<Certificate>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<DateTime>(), 
                It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
