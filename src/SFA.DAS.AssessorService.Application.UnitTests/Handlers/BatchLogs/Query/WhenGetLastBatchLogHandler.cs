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
    public class WhenGetLastBatchLogHandler
    {
        private Mock<IBatchLogQueryRepository> _batchLogQueryRepository;

        private BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = 999 };
        private BatchLogResponse _response;

        [SetUp]
        public async Task Arrange()
        {
            MappingBootstrapper.Initialize();

            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            _batchLogQueryRepository.Setup(r => r.GetLastBatchLog()).Returns(Task.FromResult(_batchLog));

            var getBatchLogHandler = new GetLastBatchLogHandler(_batchLogQueryRepository.Object);

            _response = await getBatchLogHandler.Handle(new GetLastBatchLogRequest(), new CancellationToken());
        }

        [Test]
        public void Then_response_batchlog_is_returned()
        {
            _response.Id.Should().Be(_batchLog.Id);
            _response.BatchNumber.Should().Be(_batchLog.BatchNumber);
        }
    }
}
