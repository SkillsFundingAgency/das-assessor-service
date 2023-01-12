using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.UpdateBatchLogPrintedHandlerTests
{
    public class When_called_and_batch_does_not_exist : UpdateBatchLogPrintedHandlerTestBase
    {
        private ValidationResponse _response;
        private DateTime _utcNow = DateTime.UtcNow;

        [SetUp]
        public async Task Arrange()
        {
            // Arrange
            base.BaseArrange();

            // Act
            var sut = new UpdateBatchLogPrintedHandler(_batchLogRepository.Object);
            _response = await sut.Handle(new UpdateBatchLogPrintedRequest
            {
                BatchNumber = _invalidBatchNumber,
                BatchDate = _utcNow,
                TotalCertificateCount = 2,
                PostalContactCount = 2,
                PrintedDate = _utcNow,
                DateOfResponse = _utcNow
            }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            // Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_respository_is_called_to_update_batchlog()
        {
            // Assert
            _batchLogRepository.Verify(r => r.UpdateBatchLogPrinted(
                It.Is<BatchLog>(b =>
                    b.BatchNumber == _invalidBatchNumber &&
                    b.BatchData.BatchNumber == _invalidBatchNumber &&
                    b.BatchData.BatchDate == _utcNow &&
                    b.BatchData.TotalCertificateCount == 2 &&
                    b.BatchData.PostalContactCount == 2 &&
                    b.BatchData.PrintedDate == _utcNow &&
                    b.BatchData.DateOfResponse == _utcNow)), Times.Once());
        }
    }
}
