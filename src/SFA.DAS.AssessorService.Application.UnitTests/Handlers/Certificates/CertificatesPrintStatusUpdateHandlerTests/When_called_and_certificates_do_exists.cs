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
    [TestFixture]
    public class When_called_and_certificates_do_exists : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;

        private const string CertificateReferenceFirst = "00123456";
        private Guid CertificateIdFirst = Guid.NewGuid();

        private const string CertificateReferenceSecond = "00123457";
        private Guid CertificateIdSecond = Guid.NewGuid();

        private const string CertificateReferenceThird = "00123458";
        private Guid CertificateIdThird = Guid.NewGuid();
        
        private const int BatchNumberFirst = 111;
        private const int BatchNumberSecond = 222;

        private static readonly DateTime BatchNumberFirstSentToPrinterAt = DateTime.Now.AddHours(-1);
        private static readonly DateTime BatchNumberSecondPrintedAt = DateTime.Now;

        [SetUp]
        public void Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateIdFirst, CertificateReferenceFirst, CertificateStatus.SentToPrinter, BatchNumberFirstSentToPrinterAt, BatchNumberFirst, BatchNumberFirstSentToPrinterAt.AddMinutes(-5))
                .WithCertificate(CertificateIdSecond, CertificateReferenceSecond, CertificateStatus.Printed, BatchNumberSecondPrintedAt, BatchNumberSecond, BatchNumberSecondPrintedAt.AddMinutes(-5))
                .WithCertificate(CertificateIdThird, CertificateReferenceThird, CertificateStatus.Printed, BatchNumberSecondPrintedAt, BatchNumberSecond, BatchNumberSecondPrintedAt.AddMinutes(-5))
                .WithCertificateBatchLog(BatchNumberFirst, CertificateReferenceFirst, CertificateStatus.SentToPrinter, BatchNumberFirstSentToPrinterAt, null, BatchNumberFirstSentToPrinterAt.AddMinutes(5))
                .WithCertificateBatchLog(BatchNumberSecond, CertificateReferenceSecond, CertificateStatus.Printed, BatchNumberSecondPrintedAt, null, BatchNumberSecondPrintedAt.AddMinutes(5))
                .WithCertificateBatchLog(BatchNumberSecond, CertificateReferenceThird, CertificateStatus.Printed, BatchNumberSecondPrintedAt, null, BatchNumberSecondPrintedAt.AddMinutes(5))
                .WithBatchLog(BatchNumberFirst)
                .WithBatchLog(BatchNumberSecond);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_printed()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumberFirst,
                CertificateReference = CertificateReferenceFirst,
                Status = CertificateStatus.Printed,
                StatusAt = BatchNumberFirstSentToPrinterAt.AddMinutes(30)
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
                BatchNumber = BatchNumberFirst,
                CertificateReference = CertificateReferenceFirst,
                Status = CertificateStatus.Printed,
                StatusAt = BatchNumberFirstSentToPrinterAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateIdFirst, BatchNumberFirst, request.Status, request.StatusAt, null, true, true);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumberSecond,
                CertificateReference = CertificateReferenceSecond,
                Status = CertificateStatus.Delivered,
                StatusAt = BatchNumberSecondPrintedAt.AddMinutes(30)
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
                BatchNumber = BatchNumberSecond,
                CertificateReference = CertificateReferenceSecond,
                Status = CertificateStatus.Delivered,
                StatusAt = BatchNumberSecondPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateIdSecond, BatchNumberSecond, request.Status, request.StatusAt, null, true, true);
        }

        [Test]
        public async Task Then_validation_response_is_valid_true_for_not_delivered()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumberSecond,
                CertificateReference = CertificateReferenceThird,
                Status = CertificateStatus.NotDelivered,
                StatusAt = BatchNumberSecondPrintedAt.AddMinutes(30)
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
                BatchNumber = BatchNumberSecond,
                CertificateReference = CertificateReferenceThird,
                Status = CertificateStatus.NotDelivered,
                StatusAt = BatchNumberSecondPrintedAt.AddMinutes(30)
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusCalled(CertificateIdThird, BatchNumberSecond, request.Status, request.StatusAt, null, true, true);
        }
    }
}
