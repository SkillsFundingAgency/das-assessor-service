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
    public class GetDeliveryAreasTests
    {
        private static RegisterQueryController _queryController;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private List<DeliveryArea> _expectedDeliveryAreas;
        private DeliveryArea _deliveryArea1;
        private DeliveryArea _deliveryArea2;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _deliveryArea1 = new DeliveryArea { Id = 1, Area = "Area 9", Status="Live" };
            _deliveryArea2 = new DeliveryArea { Id = 2, Area = "Area 2", Status="New" };

            _expectedDeliveryAreas = new List<DeliveryArea>
            {
                _deliveryArea1,
                _deliveryArea2
            };

            _mediator.Setup(m =>
                m.Send(It.IsAny<GetDeliveryAreasRequest>(),
                    new CancellationToken())).ReturnsAsync(_expectedDeliveryAreas);
            _queryController = new RegisterQueryController(_mediator.Object, _logger.Object);

            _result = _queryController.GetDeliveryAreas().Result;
        }

        [Test]
        public void GetDeliveryAreasReturnExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedGetDeliveryAreasRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetDeliveryAreasRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetDeliveryAreasShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test] public void ResultsAreOfTypeListDeliveryArea()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<List<DeliveryArea>>();
        }

        [Test]
        public void ResultsMatchExpectedListOfDeliveryAreas()
        {
            var deliveryAreas = ((OkObjectResult)_result).Value as List<DeliveryArea>;
            deliveryAreas.Count.Should().Be(2);
            deliveryAreas.Should().Contain(_deliveryArea1);
            deliveryAreas.Should().Contain(_deliveryArea2);
        }
    }
}
