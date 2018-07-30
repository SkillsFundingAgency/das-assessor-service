using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetOrganisationsTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<AssessmentOrganisationSummary> _expectedAssessmentOrganisationSummaries;
        private AssessmentOrganisationSummary _assOrgSummary1;
        private AssessmentOrganisationSummary _assOrgSummary2;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _assOrgSummary1 = new AssessmentOrganisationSummary { Id = "Id1", Name = "Name 9", Ukprn = 9999999 };
            _assOrgSummary2 = new AssessmentOrganisationSummary { Id = "Id2", Name = "Name 2", Ukprn = 8888888 };

            _expectedAssessmentOrganisationSummaries = new List<AssessmentOrganisationSummary>
            {
                _assOrgSummary1,
                _assOrgSummary2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSummaries);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

        _result = _queryController.GetAssessmentOrganisations().Result;
        }

        [Test]
        public void GetAssessmentOrganisationSummariesReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetAssessmentOrganisationsRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAssessmentOrganisationsRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAssessmentOrganisationsShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeListAssessmentOrganisationSummary()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AssessmentOrganisationSummary>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationSummaries()
        {
            var organisationTypes = ((OkObjectResult)_result).Value as List<AssessmentOrganisationSummary>;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_assOrgSummary1);
            organisationTypes.Should().Contain(_assOrgSummary2);
        }
    }
}
