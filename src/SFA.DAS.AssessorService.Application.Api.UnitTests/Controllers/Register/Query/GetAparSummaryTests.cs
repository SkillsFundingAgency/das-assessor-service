using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetAparSummaryTests
    {
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<RegisterQueryController>> _logger;
        private RegisterQueryController _sut;
        private object _result;

        private List<AparSummary> _expectedAparSummaries;
        private AparSummary _aparSummary1;
        private AparSummary _aparSummary2;

        [SetUp]
        public async Task Arrange()
        {
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _aparSummary1 = new AparSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddDays(-1) };
            _aparSummary2 = new AparSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222, 
                EarliestDateStandardApprovedOnRegister = DateTime.Today, EarliestEffectiveFromDate = DateTime.Today.AddMonths(-1) };

            _expectedAparSummaries = new List<AparSummary>
            {
                _aparSummary1,
                _aparSummary2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAparSummaryRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAparSummaries);
            _sut = new RegisterQueryController(_mediator.Object, _backgroundTaskQueue.Object, _logger.Object);

            _result = await _sut.GetAparSummary();
        }

        [Test]
        public void GetAparSummary_MediatorShouldSendGetAparSummaryRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAparSummaryRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAparSummary_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAparSummary_ResultsAreOfTypeListAparSummary_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AparSummary>>();
        }

        [Test]
        public void GetAparSummary_ResultsMatchExpectedListAparSummary_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AparSummary>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_aparSummary1);
            organisations.Should().Contain(_aparSummary2);
        }
    }
}
