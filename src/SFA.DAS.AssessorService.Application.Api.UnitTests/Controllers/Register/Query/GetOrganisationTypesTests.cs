using System.Collections.Generic;
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
    public class GetOrganisationTypesTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<OrganisationType> _expectedOrganisationTypes;
        private OrganisationType _organisationType1;
        private OrganisationType _organisationType2;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _organisationType1 = new OrganisationType {Id = 1, Type = "Type 1"};
            _organisationType2 = new OrganisationType {Id = 2, Type = "Another Type"};

            _expectedOrganisationTypes = new List<OrganisationType>
            {
                _organisationType1,
                _organisationType2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetOrganisationTypesRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedOrganisationTypes);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetOrganisationTypes().Result;
        }

        [Test]
        public void GetOrganisationTypesReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetOrganisationTypesRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetOrganisationTypesRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetOrganisationTypesShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeListOrganisationType()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<OrganisationType>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfOrganisationTypes()
        {
            var organisationTypes = ((OkObjectResult)_result).Value as List<OrganisationType>;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_organisationType1);
            organisationTypes.Should().Contain(_organisationType2);
        }
    }
}
