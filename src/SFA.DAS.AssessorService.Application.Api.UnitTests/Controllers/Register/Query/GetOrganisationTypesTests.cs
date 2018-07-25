using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Entities.ao;

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
                m.Send(It.IsAny<GetOrganisationsRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedOrganisationTypes);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetOrganisationTypes().Result;
        }

        [Test]
        public void GetOrganisationTypesReturnExpectedTResult()
        {
           // _result = _queryController.GetOrganisationTypes().Result;
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void GetOrganisationTypesCallsExpectedMediatorSend()
        {
           // _result = _queryController.GetOrganisationTypes().Result;
            _mediator.Verify(m => m.Send(It.IsAny<GetOrganisationsRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetOrganisationTypesShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void Then_model_should_contain_search_results()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<EpaOrganisationType>>();
        }

        [Test]
        public void Then_search_results_should_be_correct()
        {
            var organisationTypes = ((OkObjectResult)_result).Value as List<EpaOrganisationType>;
            organisationTypes.Count.Should().Be(2);
            organisationTypes.Should().Contain(_epaOrganisationType1);
            organisationTypes.Should().Contain(_epaOrganisationType2);
        }
    }
}
