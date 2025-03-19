using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Schedule.Command
{
    public class WhenUpdateLastRunStatus
    {
        private ScheduleController _sut;
        private IActionResult _result;
        private Mock<IScheduleRepository> _repository;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IScheduleRepository>();
            _sut = new ScheduleController(_repository.Object);
        }

        [Test]
        public async Task ThenShouldReturnOk()
        {
            var request = new UpdateLastRunStatusRequest { LastRunStatus = LastRunStatus.Completed, ScheduleRunId = Guid.NewGuid() };

            _result = await _sut.UpdateLastRunStatus(request);

            var result = _result as OkResult;
            result.Should().NotBeNull();
        }

        [TestCase(LastRunStatus.Failed)]
        [TestCase(LastRunStatus.Restarting)]
        [TestCase(LastRunStatus.Started)]
        public async Task ThenShouldUpdateLastRunStatus(LastRunStatus lastRunStatus)
        {
            var request = new UpdateLastRunStatusRequest { LastRunStatus = lastRunStatus, ScheduleRunId = Guid.NewGuid() };

            await _sut.UpdateLastRunStatus(request);

            _repository.Verify(x => 
            x.UpdateLastRunStatus(It.Is<UpdateLastRunStatusRequest>(y => y.ScheduleRunId == request.ScheduleRunId && y.LastRunStatus == lastRunStatus))
               ,  Times.Once());
        }
    }
}
