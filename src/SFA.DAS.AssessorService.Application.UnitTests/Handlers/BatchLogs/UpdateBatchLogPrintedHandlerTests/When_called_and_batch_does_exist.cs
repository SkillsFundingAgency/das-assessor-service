using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.UpdateBatchLogPrintedHandlerTests
{
    public class When_called_and_batch_does_exist : UpdateBatchLogPrintedHandlerTestBase
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
                BatchNumber = _validBatchNumber, 
                BatchDate = _utcNow, 
                TotalCertificateCount = 1, 
                PostalContactCount = 1, 
                PrintedDate = _utcNow, 
                DateOfResponse = _utcNow 
            }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_true()
        {
            // Assert
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public void Then_respository_is_called_to_update_batchlog()
        {
            // Assert
            _batchLogRepository.Verify(r => r.UpdateBatchLogPrinted(
                _validBatchNumber,
                It.Is<BatchData>(p =>
                    p.BatchNumber == _validBatchNumber &&
                    p.BatchDate == _utcNow &&
                    p.TotalCertificateCount == 1 &&
                    p.PostalContactCount == 1 &&
                    p.PrintedDate == _utcNow &&
                    p.DateOfResponse == _utcNow)), Times.Once());
        }
    }
}
