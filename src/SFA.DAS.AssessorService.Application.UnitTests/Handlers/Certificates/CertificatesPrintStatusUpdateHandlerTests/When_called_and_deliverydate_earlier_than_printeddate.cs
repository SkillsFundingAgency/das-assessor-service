using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    [TestFixture]
    public class When_called_and_deliverydate_earlier_than_printeddate : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;
        
        private const string CertificateReference = "00123456";
        private const int BatchNumber = 222;

        private static readonly DateTime PrintedAt = DateTime.Now;
        private static readonly DateTime DeliveredAt = PrintedAt.AddMinutes(-30);

        [SetUp]
        public async Task Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateReference, CertificateStatus.Printed, PrintedAt, BatchNumber, PrintedAt.AddMinutes(-5))
                .WithCertificateBatchLog(BatchNumber, CertificateReference, CertificateStatus.Printed, PrintedAt, null, PrintedAt.AddMinutes(5))
                .WithBatchLog(BatchNumber);

            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = DeliveredAt
            };

            // Act
            _response = await _fixture.Handle(request);
        }


        [Test]
        public void Then_validation_response_is_valid_false()
        {
            // Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_repository_update_print_status_should_not_be_called()
        {
            // Assert
            _fixture.VerifyUpdatePrintStatusNotCalled(
                CertificateReference,
                BatchNumber);
        }
    }
}
