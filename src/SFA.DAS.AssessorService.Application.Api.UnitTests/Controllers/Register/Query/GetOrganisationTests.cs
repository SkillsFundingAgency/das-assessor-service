﻿using FluentAssertions;
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
using System.Threading;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class GetOrganisationTests
    {
        private Mock<IMediator> _mediator;
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<ILogger<RegisterQueryController>> _logger;
        private RegisterQueryController _queryController;
        private object _result;

        private EpaOrganisation _expectedAssessmentOrganisationDetails;
        private const string OrganisationId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _logger = new Mock<ILogger<RegisterQueryController>>();

            _expectedAssessmentOrganisationDetails = new EpaOrganisation
            {
                OrganisationId = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456,
                OrganisationData = new OrganisationData
                {
                    LegalName = "legal name",
                    Address1 = "address 1"
                }
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetAssessmentOrganisationRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedAssessmentOrganisationDetails);
            _queryController = new RegisterQueryController(_mediator.Object, _backgroundTaskQueue.Object, _logger.Object);

            _result = _queryController.GetAssessmentOrganisation(OrganisationId).Result;
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
        public void GetAssessmentOrganisationsShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeAssessmentOrganisationDetails()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<EpaOrganisation>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationDetails()
        {
            var organisation = ((OkObjectResult)_result).Value as EpaOrganisation;
            organisation.Should().BeEquivalentTo(_expectedAssessmentOrganisationDetails);
        }
    }
}
