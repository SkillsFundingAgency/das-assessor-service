using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.PrintedBatchLogHandlerTests
{
    public class When_called_and_processed_date_earlier_than_batch_date : PrintedBatchLogHandlerTestBase
    {
        protected BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber, BatchCreated = DateTime.UtcNow.AddDays(1) };
        private ValidationResponse _response;

        [SetUp]
        public async Task Arrange()
        {
            //Arrange
            base.BaseArrange();

            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsAny<int>())).Returns(Task.FromResult(_batchLog));

            //Act
            var sut = new PrintedBatchLogHandler(_batchLogQueryRepository.Object, _certificateRepository.Object, _mediator.Object, _logger.Object);
            _response = await sut.Handle(new PrintedBatchLogRequest { BatchNumber = _batchNumber, PrintedAt = _printedAt }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            //Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
        }

        [Test]
        public void Then_certificate_update_print_status_message_not_sent()
        {            
            //Assert
            _mediator.Verify(r => r.Send(It.IsAny<UpdateCertificatesPrintStatusRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
