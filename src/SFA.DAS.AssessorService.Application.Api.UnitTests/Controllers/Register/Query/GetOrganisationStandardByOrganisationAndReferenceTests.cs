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
    public class GetOrganisationStandardByOrganisationAndReferenceTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private OrganisationStandard _expectedOrganisationStandard;
        private const string OrganisationId = "ABC123";
        private const string StandardReference = "ST1234";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _expectedOrganisationStandard = new OrganisationStandard
            {
                OrganisationId = OrganisationId,
                StandardTitle = "TITLE",
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetStandardByOrganisationAndReferenceRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedOrganisationStandard);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetOrganisationStandardByOrganisationAndReference(OrganisationId, StandardReference).Result;
        }

        [Test]
        public void GetOrganisationStandardByOrganisationAndReferenceReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetStandardByOrganisationAndReferenceRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetStandardByOrganisationAndReferenceRequest>(), new CancellationToken()));
        }

        [Test]
        public void GetOrganisationStandardByOrganisationAndReferenceReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeOrganisationStandard()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<OrganisationStandard>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationStandard()
        {
            var organisation = ((OkObjectResult)_result).Value as OrganisationStandard;
            organisation.Should().BeEquivalentTo(_expectedOrganisationStandard);
        }
    }
}
