using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.PrintedBatchLogHandlerTests
{
    public class When_called_and_batch_does_not_exist : PrintedBatchLogHandlerTestBase
    {        
        private ValidationResponse _response = new ValidationResponse();

        [SetUp]
        public async Task Arrange()
        {
            //Arrange
            base.BaseArrange();

            //Act
            var sut = new PrintedBatchLogHandler(_batchLogQueryRepository.Object, _certificateRepository.Object, _mediator.Object, _logger.Object);
            _response = await sut.Handle(new PrintedBatchLogRequest { BatchNumber = _batchNumber + 999, PrintedAt = _printedAt }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            //Assert
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
            _response.Errors[0].Field.Should().Be("BatchNumber");
        }
    }
}
