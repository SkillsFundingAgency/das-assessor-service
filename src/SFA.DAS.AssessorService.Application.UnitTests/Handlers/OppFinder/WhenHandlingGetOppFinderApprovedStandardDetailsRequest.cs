using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OppFinder
{
    public class WhenHandlingGetOppFinderApprovedStandardDetailsRequest
    {
        private Mock<ILogger<GetOppFinderApprovedStandardDetailsHandler>> _mockLogger;
        private Mock<IOppFinderRepository> _mockOppFinderRepository;
        private Mock<IMediator> _mockMediator;

        private GetOppFinderApprovedStandardDetailsHandler _sut;

        [SetUp]
        public void Setup()
        {
            _mockOppFinderRepository = new Mock<IOppFinderRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<GetOppFinderApprovedStandardDetailsHandler>>();

            _mockMediator.Setup(s => s.Send(It.Is<ValidationRequest>(x => x.Type == "email" && x.Value == "EMAIL"), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactEmail = "EMAIL"
                },
                RegionResults = new List<OppFinderApprovedStandardRegionResult>()
                {
                    new OppFinderApprovedStandardRegionResult()
                    {
                        Region = "REGION1"
                    },
                    new OppFinderApprovedStandardRegionResult()
                    {
                        Region = "REGION2"
                    }
                },
                VersionResults = new List<OppFinderApprovedStandardVersionResult>()
                {
                    new OppFinderApprovedStandardVersionResult()
                    {
                        Version = 1.0m,
                    },
                    new OppFinderApprovedStandardVersionResult()
                    {
                        Version = 1.1m,
                    }

                }
            });

            _sut = new GetOppFinderApprovedStandardDetailsHandler(_mockLogger.Object, _mockOppFinderRepository.Object, _mockMediator.Object);
        }

        [Test]
        public async Task ThenReturnsStandardDetails()
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.StandardCode.Should().Equals(999);
            result.StandardReference.Should().Equals("ST0001");
        }

        [Test]
        public async Task ThenReturnsEmails()
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.EqaProvider.Should().Equals("EMAIL");
            result.EqaProviderLink.Should().Equals($"mailto:EMAIL");
        }

        [Test]
        public async Task ThenReturnsRegionDetails()
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.RegionResults.Count.Should().Equals(2);
            result.RegionResults[0].Region.Should().Equals("REGION1");
            result.RegionResults[1].Region.Should().Equals("REGION2");
        }

        [Test]
        public async Task ThenReturnsVersionDetails()
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            //Act
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.VersionResults.Count.Should().Equals(2);
            result.VersionResults[0].Version.Should().Equals(1.0m);
            result.VersionResults[1].Version.Should().Equals(1.1m);
        }
    }
}
