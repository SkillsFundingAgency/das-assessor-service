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
    public class When_called_and_certificates_updated : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;

        private const string CertificateReference = "00123456";
        private const int PreviousBatchNumber = 111;
        private const int CurrentBatchNumber = 222;
        
        private static readonly DateTime PreviousBatchSentToPrinterAt = DateTime.Now.AddHours(-1);
        private static readonly DateTime CurrentBatchPrintedAt = DateTime.Now;
        
        [SetUp]
        public void Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateReference, CertificateStatus.Printed, CurrentBatchPrintedAt, CurrentBatchNumber, CurrentBatchPrintedAt.AddMinutes(-5))
                .WithCertificateBatchLog(CurrentBatchNumber, CertificateReference, CertificateStatus.Printed, CurrentBatchPrintedAt, null, CurrentBatchPrintedAt.AddMinutes(5))
                .WithCertificateBatchLog(PreviousBatchNumber, CertificateReference, CertificateStatus.SentToPrinter, PreviousBatchSentToPrinterAt, null, PreviousBatchSentToPrinterAt.AddMinutes(5))
                .WithBatchLog(CurrentBatchNumber)
                .WithBatchLog(PreviousBatchNumber);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_printed()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = PreviousBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Printed,
                StatusAt = PreviousBatchSentToPrinterAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);
            
            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_printed()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = PreviousBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Printed,
                StatusAt = PreviousBatchSentToPrinterAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateReference, PreviousBatchNumber, request.Status, request.StatusAt, null, false, true);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_not_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = CurrentBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.NotDelivered,
                StatusAt = CurrentBatchPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_not_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = CurrentBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.NotDelivered,
                StatusAt = CurrentBatchPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateReference, CurrentBatchNumber, request.Status, request.StatusAt, null, true, true);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = CurrentBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = CurrentBatchPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_called_for_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = CurrentBatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = CurrentBatchPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateReference, CurrentBatchNumber, request.Status, request.StatusAt, null, true, true);
        }
    }
}
