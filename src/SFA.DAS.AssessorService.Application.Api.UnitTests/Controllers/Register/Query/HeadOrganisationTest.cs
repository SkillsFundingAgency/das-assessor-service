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
    public class HeadOrganisationTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private EpaOrganisation _expectedAssessmentOrganisationDetails;
        private const string OrganisationId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _expectedAssessmentOrganisationDetails = new EpaOrganisation
            {
                OrganisationId = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456
            };


            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationDetails);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.Head(OrganisationId).Result;
        }

        [Test]
        public void GetAssessmentOrganisationSummariesReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetAssessmentOrganisationRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAssessmentOrganisationRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAssessmentOrganisationsShouldReturnNoContent()
        {
            _result.Should().BeOfType<NoContentResult>();
        }
    }
}
