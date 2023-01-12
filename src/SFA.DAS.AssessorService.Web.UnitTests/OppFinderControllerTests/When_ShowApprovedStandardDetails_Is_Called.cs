using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.OppFinder;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.OppFinder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OppFinderControllerTests
{
    [TestFixture]
    public class When_ShowApprovedStandardDetails_Is_Called
    {
        private Mock<IOppFinderSession> _mockOppFinderSession;
        private Mock<IOppFinderApiClient> _mockOppFinderApiClient;
        private Mock<IValidationApiClient> _mockValidationApiClient;
        private Mock<ILogger<OppFinderController>> _mockLogger;

        private OppFinderController _sut;

        [SetUp]
        public void Arrange()
        {
            _mockOppFinderSession = new Mock<IOppFinderSession>();
            _mockOppFinderApiClient = new Mock<IOppFinderApiClient>();
            _mockValidationApiClient = new Mock<IValidationApiClient>();
            _mockLogger = new Mock<ILogger<OppFinderController>>();

            _sut = new OppFinderController(_mockOppFinderSession.Object, _mockOppFinderApiClient.Object, _mockValidationApiClient.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Then_Details_For_Standard_Are_Returned()
        {
            // Arrange
            _mockOppFinderApiClient.Setup(m => m.GetApprovedStandardDetails(It.Is<GetOppFinderApprovedStandardDetailsRequest>(x => x.StandardReference == "ST0001")))
                                    .ReturnsAsync(new GetOppFinderApprovedStandardDetailsResponse()
                                    {
                                        StandardCode = 999,
                                        StandardReference = "ST0001",
                                        RegionResults = new List<OppFinderApprovedStandardDetailsRegionResult>()
                                        {
                                            new OppFinderApprovedStandardDetailsRegionResult()
                                            {
                                                Region = "REGION1"
                                            },
                                            new OppFinderApprovedStandardDetailsRegionResult()
                                            {
                                                Region = "REGION2"
                                            }
                                        },
                                        VersionResults = new List<OppFinderApprovedStandardDetailsVersionResult>()
                                        {
                                            new OppFinderApprovedStandardDetailsVersionResult()
                                            {
                                                Version = "1.0",
                                            },
                                            new OppFinderApprovedStandardDetailsVersionResult()
                                            {
                                                Version = "1.1",
                                            }
                                        }
                                    });

            // Act
            var results = (await _sut.ShowApprovedStandardDetails("ST0001")) as ViewResult;

            // Assert
            var vm = results.Model as OppFinderApprovedDetailsViewModel;
            vm.StandardCode.Should().Equals(999);
            vm.StandardReference.Should().Equals("ST0001");

            vm.RegionResults.Count.Should().Equals(2);
            vm.RegionResults[0].Region.Should().Equals("REGION1");
            vm.RegionResults[1].Region.Should().Equals("REGION2");

            vm.VersionResults.Count.Should().Equals(2);
            vm.VersionResults[0].Version.Should().Equals("1.0");
            vm.VersionResults[1].Version.Should().Equals("1.1");
        }
    }
}
