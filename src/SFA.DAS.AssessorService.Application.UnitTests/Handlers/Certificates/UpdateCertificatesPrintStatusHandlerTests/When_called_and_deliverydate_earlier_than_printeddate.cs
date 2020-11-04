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
    public class When_called_and_deliverydate_earlier_than_printeddate : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        /*private ValidationResponse _response;
        private static DateTime _statusChangedAt = DateTime.UtcNow;

        [SetUp]
        public async Task Arrange()
        {
            //Arrange
            base.BaseArrange();

            var certificatePrintStatuses = new List<CertificatePrintStatus>
            {                
                new CertificatePrintStatus
                {
                    BatchNumber = _batchNumber,
                    CertificateReference = _certificateReference5,
                    Status = CertificateStatus.Delivered,
                    StatusChangedAt = _statusChangedAt
                }
            };

            //Act
            _response = await _sut.Handle(
                new UpdateCertificatesPrintStatusRequest
                {
                    CertificatePrintStatuses = certificatePrintStatuses
                }, new CancellationToken());
        }


        [Test]
        public void Then_validation_response_is_valid_false()
        {
            //Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_repository_update_print_status_is_called()
        {          
            //Assert
            _certificateRepository.Verify(r => r.UpdatePrintStatus(
                It.Is<Certificate>(c => c.CertificateReference == _certificateReference5), _batchNumber, CertificateStatus.Delivered, _statusChangedAt, null, false),
                Times.Once);
        }*/
    }
}
