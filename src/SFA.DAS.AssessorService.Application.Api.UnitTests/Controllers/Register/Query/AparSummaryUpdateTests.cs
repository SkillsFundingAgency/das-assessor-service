using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    public class AparSummaryUpdateTests
    {
        private static RegisterQueryController _sut;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private static object _result;
        private static DateTime _expectedLastUpdated;

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _expectedLastUpdated = DateTime.UtcNow;

            _mediator.Setup(m => m.Send(It.IsAny<AparSummaryLastUpdatedRequest>(), new CancellationToken()))
                .ReturnsAsync(_expectedLastUpdated);

            _sut = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = await _sut.AparSummaryLastUpdated();
        }

        [Test]
        public void GetAparSummary_MediatorShouldSendGetAparSummaryRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<AparSummaryLastUpdatedRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAparSummary_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAparSummary_ResultsAreOfTypeListAssessmentOrganisationListSummary_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<DateTime>();
        }

        [Test]
        public void GetAparSummary_ResultsMatchExpectedListAssessmentOrganisationListSummary_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as DateTime?;
            organisations.Should().Be(_expectedLastUpdated);
        }
    }
}
