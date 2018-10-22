using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class SearchStandardsBySearchStringTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<StandardSummary> _expectedStandards;
        private StandardSummary _standard1;
        private StandardSummary _standard2;
        private string _searchString = "Test";
        
        
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _standard1 = new StandardSummary { Id = "Id1", Title = "Test 9"};
            _standard2 = new StandardSummary  {Id = "Id2", Title = "Test 2"};
            _expectedStandards = new List<StandardSummary>
            {
                _standard1,
                _standard2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<SearchStandardsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedStandards);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);
            _result = _queryController.SearchStandards(_searchString).Result;
        }  

        [Test]
        public void SearchStandardsBySearchstringReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }
        [Test]
        public void MediatorSendsExpectedSearchStandardsBySsarchstringRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<SearchStandardsRequest>(), new CancellationToken()));
        }
        [Test]
        public void SearchAssessmentOrganisationsReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }
        [Test]
        public void ResultsAreOfTypeListAssessmentOrganisationDetails()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<StandardSummary>>();
        }
        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationDetails()
        {
            var standards = ((OkObjectResult)_result).Value as List<StandardSummary>;
            standards.Count.Should().Be(2);
            standards.Should().Contain(_standard1);
            standards.Should().Contain(_standard2);
        }
    }
}