using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class SearchOrganisationBySearchStringTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<AssessmentOrganisationSummary> _expectedAssessmentOrganisationSetOfDetails;
        private AssessmentOrganisationSummary _assOrgDetails1;
        private AssessmentOrganisationSummary _assOrgDetails2;
        private string searchString = "Test";


        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            // needs more details
            _assOrgDetails1 = new AssessmentOrganisationSummary { Id = "Id1", Name = "Test 9", Ukprn = 9999999 };
            _assOrgDetails2 = new AssessmentOrganisationSummary  {Id = "Id2", Name = "Test 2", Ukprn = 8888888 };
            _expectedAssessmentOrganisationSetOfDetails = new List<AssessmentOrganisationSummary>
            {
                _assOrgDetails1,
                _assOrgDetails2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<SearchAssessmentOrganisationsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSetOfDetails);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);
            _result = _queryController.SearchAssessmentOrganisations(searchString).Result;
        }


        [Test]
        public void SearchAssessmentOrganisationsByStandardIdReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }
        [Test]
        public void MediatorSendsExpectedSearchAssessmentOrganisationsBySsarchstringRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<SearchAssessmentOrganisationsRequest>(), new CancellationToken()));
        }
        [Test]
        public void SearchAssessmentOrganisationsReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }
        [Test]
        public void ResultsAreOfTypeListAssessmentOrganisationDetails()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AssessmentOrganisationSummary>>();
        }
        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationDetails()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AssessmentOrganisationSummary>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assOrgDetails1);
            organisations.Should().Contain(_assOrgDetails2);
        }
    }
}
