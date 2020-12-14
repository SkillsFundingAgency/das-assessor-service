using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.UpdateBatchLogSentToPrinterHandlerTests
{
    public class When_called_and_batch_does_not_exist : UpdateBatchLogSentToPrinterHandlerTestBase
    {
        private ValidationResponse _response;
        private DateTime _utcNow = DateTime.UtcNow;

        [SetUp]
        public async Task Arrange()
        {
            // Arrange
            base.BaseArrange();

            // Act
            var sut = new UpdateBatchLogSentToPrinterHandler(_batchLogRepository.Object);
            _response = await sut.Handle(new UpdateBatchLogSentToPrinterRequest
            {
                BatchNumber = _invalidBatchNumber,
                BatchCreated = _utcNow,
                NumberOfCertificates = 1,
                NumberOfCoverLetters = 1,
                CertificatesFileName = "TestFileName",
                FileUploadStartTime = _utcNow,
                FileUploadEndTime = _utcNow
            }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_true()
        {
            // Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_respository_is_called_to_update_batchlog()
        {
            // Assert
            _batchLogRepository.Verify(r => r.UpdateBatchLogSentToPrinter(
                It.Is<BatchLog>(b =>
                    b.BatchNumber == _invalidBatchNumber &&
                    b.BatchCreated == _utcNow &&
                    b.NumberOfCertificates == 1 &&
                    b.NumberOfCoverLetters == 1 &&
                    b.CertificatesFileName == "TestFileName" &&
                    b.FileUploadStartTime == _utcNow &&
                    b.FileUploadEndTime == _utcNow &&
                    b.BatchData.BatchNumber == _invalidBatchNumber)), Times.Once);
        }
    }
}
