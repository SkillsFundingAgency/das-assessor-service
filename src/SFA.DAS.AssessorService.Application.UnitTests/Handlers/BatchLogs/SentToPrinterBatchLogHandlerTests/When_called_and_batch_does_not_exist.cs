using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.SentToPrinterBatchLogHandlerTests
{
    public class When_called_and_batch_does_not_exist
    {
        /*private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
        private Mock<ILogger<SentToPrinterBatchLogHandler>> _logger;

        private static int _batchNumber = 222;
        private static List<string> _certificateReferences = new List<string> { };

        private BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber };
        private ValidationResponse _response = new ValidationResponse();

        [SetUp]
        public async Task Arrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            
            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsIn(_batchNumber))).Returns(Task.FromResult(_batchLog));
            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsNotIn(_batchNumber))).Returns(Task.FromResult<BatchLog>(null));

            _logger = new Mock<ILogger<SentToPrinterBatchLogHandler>>();

            var sut = new SentToPrinterBatchLogHandler(_certificateRepository.Object, _batchLogQueryRepository.Object, _logger.Object);

            _response = await sut.Handle(new SentToPrinterBatchLogRequest { BatchNumber = _batchNumber + 999, CertificateReferences = _certificateReferences }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_false()
        {
            _response.IsValid.Should().Be(false);
            _response.Errors.Count.Should().Be(1);
            _response.Errors[0].Field.Should().Be("BatchNumber");
        }*/
    }
}
