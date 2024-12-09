using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.Query
{
    public class WhenGetBatchLogHandler : MapperBase
    {
        private Mock<IBatchLogQueryRepository> _batchLogQueryRepository;

        private static int _batchNumber = 222;
        private BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber };
        private BatchLogResponse _response;

        [SetUp]
        public async Task Arrange()
        {

            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            _batchLogQueryRepository.Setup(r => r.Get(It.IsAny<int>())).Returns(Task.FromResult(_batchLog));

            var sut = new GetBatchLogHandler(_batchLogQueryRepository.Object, Mapper);

            _response = await sut.Handle(new GetBatchLogRequest { BatchNumber = _batchNumber }, new CancellationToken());
        }

        [Test]
        public void Then_response_batchlog_is_returned()
        {
            _response.Id.Should().Be(_batchLog.Id);
            _response.BatchNumber.Should().Be(_batchLog.BatchNumber);
        }
    }
}
