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
    public class When_called_and_status_not_print_status : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;

        private const string CertificateReference = "00123456";
        private const int BatchNumber = 111;

        private static readonly DateTime SentToPrinterAt = DateTime.UtcNow.AddHours(-1);

        [SetUp]
        public void Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateReference, CertificateStatus.SentToPrinter, SentToPrinterAt, BatchNumber, SentToPrinterAt.AddMinutes(-5))
                .WithCertificateBatchLog(BatchNumber, CertificateReference, CertificateStatus.SentToPrinter, SentToPrinterAt, null, SentToPrinterAt.AddMinutes(5))
                .WithBatchLog(BatchNumber);
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
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = invalidPrintStatus,
                StatusAt = DateTime.UtcNow
            };

            // Act
            _response = await _fixture.Handle(request);

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
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = invalidPrintStatus,
                StatusAt = DateTime.UtcNow
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusNotCalled(CertificateReference, BatchNumber);
        }
    }
}
