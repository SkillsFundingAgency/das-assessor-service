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
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class SearchStandardsBySearchTermTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<StandardCollation> _expectedStandards;
        private StandardCollation _standard1;
        private StandardCollation _standard2;
        private string _searchTerm = "Test";
        
        
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _standard1 = new StandardCollation {StandardId = 1, Title = "Test 9"};
            _standard2 = new StandardCollation {StandardId = 1, Title = "Test 2"};
            _expectedStandards = new List<StandardCollation>
            {
                _standard1,
                _standard2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<SearchStandardsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedStandards);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);
            _result = _queryController.SearchStandards(_searchTerm).Result;
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
            ((OkObjectResult)_result).Value.Should().BeOfType<List<StandardCollation>>();
        }
        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationDetails()
        {
            var standards = ((OkObjectResult)_result).Value as List<StandardCollation>;
            standards.Count.Should().Be(2);
            standards.Should().Contain(_standard1);
            standards.Should().Contain(_standard2);
        }
    }
}