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
    public class When_called_and_certificate_does_not_exist_in_the_batch : UpdateCertificatesPrintStatusHandlerTestsBase
    {
        private CertificatePrintStatusUpdateHandlerTestsFixture _fixture;
        private ValidationResponse _response;

        private const string CertificateReferenceExists = "00123456";
        private Guid CertificateIdExists = Guid.NewGuid();

        private const string CertificateReferenceNotExists = "00123457";
        private Guid CertificateIdNotExists = Guid.NewGuid();

        private const int BatchNumber = 111;

        private static readonly DateTime SentToPrinterAt = DateTime.UtcNow.AddHours(-1);

        [SetUp]
        public void Arrange()
        {
            // Arrange
            _fixture = new CertificatePrintStatusUpdateHandlerTestsFixture()
                .WithCertificate(CertificateIdExists, CertificateReferenceExists, CertificateStatus.SentToPrinter, SentToPrinterAt, BatchNumber, SentToPrinterAt.AddMinutes(-5))
                .WithCertificateBatchLog(BatchNumber, CertificateReferenceExists, CertificateStatus.SentToPrinter, SentToPrinterAt, null, SentToPrinterAt.AddMinutes(5))
                .WithBatchLog(BatchNumber);
        }

        [Test]
        public async Task Then_validation_response_is_valid_false()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReferenceNotExists,
                Status = CertificateStatus.Printed,
                StatusAt = DateTime.UtcNow
            };

            // Act
            _response = await _fixture.Handle(request);

            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);

            _response.Errors[0].Field.Should().Be("CertificateReference");
            _response.Errors[0].ErrorMessage.Should().Contain(CertificateReferenceNotExists);
        }

        [Test]
        public async Task Then_repository_update_print_status_is_not_called()
        {
            // Arrange
            var request = new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReferenceNotExists,
                Status = CertificateStatus.Printed,
                StatusAt = DateTime.UtcNow
            };

            // Act
            _response = await _fixture.Handle(request);

            // Assert
            _fixture.VerifyUpdatePrintStatusNotCalled(CertificateIdNotExists, BatchNumber);
        }
    }
}
