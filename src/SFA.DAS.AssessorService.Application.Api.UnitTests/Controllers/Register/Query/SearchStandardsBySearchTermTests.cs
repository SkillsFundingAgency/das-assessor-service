using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using DomainStandard = SFA.DAS.AssessorService.Domain.Entities.Standard;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class SearchStandardsBySearchTermTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<ILogger<RegisterQueryController>> _logger;
        private RegisterQueryController _queryController;
        private object _result;

        private List<DomainStandard> _expectedStandards;
        private DomainStandard _standard1;
        private DomainStandard _standard2;
        private string _searchTerm = "Test";
        
        
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _standard1 = new Standard {LarsCode = 1, Title = "Test 9"};
            _standard2 = new Standard {LarsCode = 1, Title = "Test 2"};
            _expectedStandards = new List<Standard>
            {
                _standard1,
                _standard2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<SearchStandardsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedStandards);
            _queryController = new RegisterQueryController(_mediator.Object, _backgroundTaskQueue.Object, _logger.Object);
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
            ((OkObjectResult)_result).Value.Should().BeOfType<List<StandardVersion>>();
        }
        [Test]
        public void ResultsMatchExpectedListOfStandards()
        {
            var standards = ((OkObjectResult)_result).Value as List<StandardVersion>;
            standards.Count.Should().Be(2);
            standards[0].Should().BeEquivalentTo(_standard1, options => options.Excluding(s => s.IfateReferenceNumber));
            standards[1].Should().BeEquivalentTo(_standard2, options => options.Excluding(s => s.IfateReferenceNumber));
        }


    }
}