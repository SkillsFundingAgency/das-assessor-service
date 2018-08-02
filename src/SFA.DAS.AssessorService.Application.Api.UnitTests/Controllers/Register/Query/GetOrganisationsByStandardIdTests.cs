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
    public class GetOrganisationsByStandardIdTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<AssessmentOrganisationDetails> _expectedAssessmentOrganisationSetOfDetails;
        private AssessmentOrganisationDetails _assOrgDetails1;
        private AssessmentOrganisationDetails _assOrgDetails2;
        private int standardId = 1;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            // needs more details
            _assOrgDetails1 = new AssessmentOrganisationDetails { Id = "Id1", Name = "Name 9", Ukprn = 9999999 };
            _assOrgDetails2 = new AssessmentOrganisationDetails { Id = "Id2", Name = "Name 2", Ukprn = 8888888 };

            _expectedAssessmentOrganisationSetOfDetails = new List<AssessmentOrganisationDetails>
            {
                _assOrgDetails1,
                _assOrgDetails2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationsbyStandardRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationSetOfDetails);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetAssessmentOrganisationsByStandard(standardId).Result;
        }

        [Test]
        public void GetAssessmentOrganisationsByStandardIdReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetAssessmentOrganisationsByStandardIdRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAssessmentOrganisationsbyStandardRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAssessmentOrganisationsByStandardIdShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeListAssessmentOrganisationDetails()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<AssessmentOrganisationDetails>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfAssessmentOrganisationDetails()
        {
            var organisations = ((OkObjectResult)_result).Value as List<AssessmentOrganisationDetails>;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assOrgDetails1);
            organisations.Should().Contain(_assOrgDetails2);
        }
    }
}
