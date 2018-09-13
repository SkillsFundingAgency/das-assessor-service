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
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class GetStandardsByOrganisationIdTest
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<OrganisationStandardSummary> _expectedStandards;
        private OrganisationStandardSummary _standard1;
        private OrganisationStandardSummary _standard2;
        private const string OrganisationId = "EPA987";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _standard1 = new OrganisationStandardSummary { OrganisationId = OrganisationId, StandardCode = 1, Periods = new List<OrganisationStandardPeriod>()};
            _standard2 = new OrganisationStandardSummary { OrganisationId = OrganisationId, StandardCode= 2, Periods = new List<OrganisationStandardPeriod>()};

            _expectedStandards = new List<OrganisationStandardSummary>
            {
                _standard1,
                _standard2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetStandardsByOrganisationRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedStandards);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetOrganisationStandardsByOrganisation(OrganisationId).Result;
        }


        [Test]
        public void GetStandardsByOrganisationIdReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetStandardsByOrganisationIdRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetStandardsByOrganisationRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetStandardsByOrganisationIdShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeListOrganisationStandards()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<OrganisationStandardSummary>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationDetails()
        {
            var organisations = ((OkObjectResult)_result).Value as List<OrganisationStandardSummary>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_standard1);
            organisations.Should().Contain(_standard2);
        }
    }
}
