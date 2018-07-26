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
        private List<EpaOrganisationType> _expectedOrganisationTypes;
        private EpaOrganisationType _epaOrganisationType1;
        private EpaOrganisationType _epaOrganisationType2;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _epaOrganisationType1 = new EpaOrganisationType {Id = 1, OrganisationType = "Type 1"};
            _epaOrganisationType2 = new EpaOrganisationType {Id = 2, OrganisationType = "Another Type"};

            _expectedOrganisationTypes = new List<EpaOrganisationType>
            {
                _epaOrganisationType1,
                _epaOrganisationType2
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
        public void ResultsAreOfTypeListEpaOrganisationType()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<EpaOrganisationType>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfEpaOrganisationTypes()
        {
            var organisationTypes = ((OkObjectResult)_result).Value as List<EpaOrganisationType>;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_epaOrganisationType1);
            organisationTypes.Should().Contain(_epaOrganisationType2);
        }
    }
}
