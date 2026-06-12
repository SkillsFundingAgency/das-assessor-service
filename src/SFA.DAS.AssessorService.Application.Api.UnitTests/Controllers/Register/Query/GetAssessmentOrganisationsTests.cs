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
using System.Collections.Generic;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetAssessmentOrganisationsTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<ILogger<RegisterQueryController>> _logger;
        private RegisterQueryController _sut;
        private object _result;
        
        private List<AssessmentOrganisationSummary> _expectedAssessmentOrganisationSummaries;
        private AssessmentOrganisationSummary _assOrgSummary1;
        private AssessmentOrganisationSummary _assOrgSummary2;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _assOrgSummary1 = new AssessmentOrganisationSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111 };
            _assOrgSummary2 = new AssessmentOrganisationSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222 };

            _expectedAssessmentOrganisationSummaries = new List<AssessmentOrganisationSummary>
            {
                _assOrgSummary1,
                _assOrgSummary2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSummaries);
            _sut = new RegisterQueryController(_mediator.Object, _backgroundTaskQueue.Object, _logger.Object);

            _result = _sut.GetAssessmentOrganisations().Result;
        }

        [Test]
        public void GetAssessmentOrganisations_ReturnExpectedActionResult_WhenCalled()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void GetAssessmentOrganisations_MediatorShouldSendGetAssessmentOrganisationsRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAssessmentOrganisationsRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAssessmentOrganisations_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAssessmentOrganisations_ResultsAreOfTypeListAssessmentOrganisationSummary_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AssessmentOrganisationSummary>>();
        }

        [Test]
        public void GetAssessmentOrganisations_ResultsMatchExpectedListOfAssessmentOrganisationSummary_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AssessmentOrganisationSummary>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assOrgSummary1);
            organisations.Should().Contain(_assOrgSummary2);
        }
    }
}
