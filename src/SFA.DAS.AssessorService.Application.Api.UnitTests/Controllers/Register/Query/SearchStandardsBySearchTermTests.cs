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
using FluentAssertions.Equivalency;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class SearchStandardsBySearchTermTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<Standard> _expectedStandards;
        private Standard _standard1;
        private Standard _standard2;
        private string _searchTerm = "Test";
        
        
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
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
            ((OkObjectResult)_result).Value.Should().BeOfType<List<StandardVersion>>();
        }
        [Test]
        public void ResultsMatchExpectedListOfStandards()
        {
            var standards = ((OkObjectResult)_result).Value as List<StandardVersion>;
            standards.Count.Should().Be(2);
            standards[0].Should().BeEquivalentTo(_standard1, StandardEquivalencyAssertionOptions);
            standards[1].Should().BeEquivalentTo(_standard2, StandardEquivalencyAssertionOptions);
        }

        private EquivalencyAssertionOptions<Standard> StandardEquivalencyAssertionOptions(EquivalencyAssertionOptions<Standard> options)
        {
            return options.Excluding(x => x.IfateReferenceNumber)
                .Excluding(x => x.VersionMajor)
                .Excluding(x => x.VersionMinor)
                .Excluding(x => x.Status)
                .Excluding(x => x.TypicalDuration)
                .Excluding(x => x.MaxFunding)
                .Excluding(x => x.IsActive)
                .Excluding(x => x.LastDateStarts)
                .Excluding(x => x.VersionApprovedForDelivery)
                .Excluding(x => x.ProposedMaxFunding)
                .Excluding(x => x.ProposedTypicalDuration)
                .Excluding(x => x.TrailBlazerContact)
                .Excluding(x => x.Route)
                .Excluding(x => x.IntegratedDegree)
                .Excluding(x => x.EqaProviderName)
                .Excluding(x => x.EqaProviderContactName)
                .Excluding(x => x.EqaProviderContactEmail)
                .Excluding(x => x.OverviewOfRole);
        }
    }
}