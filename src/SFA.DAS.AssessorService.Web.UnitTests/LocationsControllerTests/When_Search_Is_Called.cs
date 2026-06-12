using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;

namespace SFA.DAS.AssessorService.Web.UnitTests.LocationsControllerTests
{
    [TestFixture]
    public class When_Search_Is_Called
    {
        private LocationsController _sut;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILocationsApiClient> _mockLocationsApiClient;
        private Mock<ILogger<LocationsController>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockLocationsApiClient = new Mock<ILocationsApiClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<LocationsController>>();

            _mockLocationsApiClient
                .Setup(r => r.SearchLocations(It.IsAny<string>()))
                .ReturnsAsync(new List<AddressResponse>
                {
                    new AddressResponse
                    {
                        Postcode = "CV1 1RF"
                    },
                    new AddressResponse
                    {
                        Postcode = "B1 6HE"
                    }
                });

            _sut = new LocationsController(_mockHttpContextAccessor.Object, _mockLocationsApiClient.Object, _mockLogger.Object);
        }

        [Test]
        public async Task The_Location_Data_Is_Returned_Correctly()
        {
            // Arrange

            // Act
            var results = await _sut.Index("Search", true);

            // Assert
            _mockLocationsApiClient.Verify(m => m.SearchLocations("Search"), Times.Once);
            results.Count.Should().Be(2);
        }
    }
}
