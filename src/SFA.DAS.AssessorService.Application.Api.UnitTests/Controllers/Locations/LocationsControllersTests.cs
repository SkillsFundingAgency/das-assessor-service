using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Locations
{
    [TestFixture]
    public class LocationsControllersTests
    {
        private Mock<IMediator> _mediator;
        private LocationsController _locationsController;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _locationsController = new LocationsController(Mock.Of<ILogger<LocationsController>>(), _mediator.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingLocationsSearch_AndQueryIsValid_ThenSearchResultsAreReturned(
            string query,
            List<AddressResponse> addresses)
        {
            //Arrange
            _mediator.Setup(s => s.Send(It.IsAny<GetAddressesRequest>(), new CancellationToken())).ReturnsAsync(addresses);
          
            //Act
            var controllerResult = await _locationsController.SearchLocations(query) as ObjectResult;

            //Assert
            _mediator.Verify(a => a.Send(It.Is<GetAddressesRequest>(b => b.Query == query), new CancellationToken()), Times.Once);
            controllerResult.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)controllerResult).Value.Should().BeOfType<List<AddressResponse>>();
            ((OkObjectResult)controllerResult).Value.Should().BeEquivalentTo(addresses);
        }

        [Test, MoqAutoData]
        public async Task WhenCallingLocationsSearch_AndQueryIsInvalid_ThenBadRequestIsReturned(
            List<AddressResponse> addresses)
        {
            //Arrange
            _mediator.Setup(s => s.Send(It.IsAny<GetAddressesRequest>(), new CancellationToken())).ReturnsAsync(addresses);

            //Act
            var controllerResult = await _locationsController.SearchLocations(null) as ObjectResult;

            //Assert
            controllerResult.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}