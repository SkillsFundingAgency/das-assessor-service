using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Validation;
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
        public async Task SetupAsync()
        {
            _mockOppFinderRepository = new Mock<IOppFinderRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<GetOppFinderApprovedStandardDetailsHandler>>();
            var validationRequest = new ValidationRequest()
            {
                Type = "email",
                Value = "EMAIL",
            };

            ValidationHandler validationHandler = new ValidationHandler(new ValidationService());
            _mockMediator.Setup(s => s.Send(It.Is<ValidationRequest>(x => x.Type == validationRequest.Type && x.Value == validationRequest.Value), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(await validationHandler.Handle(validationRequest, It.IsAny<CancellationToken>()));

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactEmail = validationRequest.Value
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
                        Version = "1.0",
                    },
                    new OppFinderApprovedStandardVersionResult()
                    {
                        Version = "1.1",
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
            using (new AssertionScope())
            {
                result.StandardCode.Should().Be(999);
                result.StandardReference.Should().Be("ST0001");
            }
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
            result.RegionResults.Count.Should().Be(2);
            using (new AssertionScope())
            {
                result.RegionResults[0].Region.Should().Be("REGION1");
                result.RegionResults[1].Region.Should().Be("REGION2");
            }
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
            result.VersionResults.Count.Should().Be(2);
            using (new AssertionScope())
            {
                result.VersionResults[0].Version.Should().Be("1.0");
                result.VersionResults[1].Version.Should().Be("1.1");
            }
        }

        [Test]
        [TestCase("eqaProviderContactEmail@email.com", "eqaProviderContactEmail@email.com", "", "OFQUAL", "eqaProviderContactEmail@email.com")]
        [TestCase("eqaProviderContactName@email.com", "", "eqaProviderContactName@email.com", "OFQUAL", "eqaProviderContactName@email.com")]
        [TestCase("", "", "", "OFQUAL", "OFQUAL")]
        [TestCase("eqaProviderContactName", "", "eqaProviderContactName", "OFQUAL", "OFQUAL")]
        [TestCase("eqaProviderContactEmail", "eqaProviderContactEmail", "", "OFQUAL", "OFQUAL")]
        public async Task ThenReturnsEqaProvider(string validationRequestEmail, string eqaProviderContactEmail, string eqaProviderContactName, string eqaProviderName, string eqaProvider)
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            var validationRequest = new ValidationRequest()
            {
                Type = "email",
                Value = validationRequestEmail
            };

            ValidationHandler validationHandler = new ValidationHandler(new ValidationService());

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactEmail = eqaProviderContactEmail,
                    EqaProviderContactName = eqaProviderContactName,
                    EqaProviderName = eqaProviderName
                },
            });

          
            _mockMediator.Setup(s => s.Send(It.Is<ValidationRequest>(x => x.Type == validationRequest.Type && x.Value == validationRequest.Value), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(await validationHandler.Handle(validationRequest, It.IsAny<CancellationToken>()));

            //Act

            _sut = new GetOppFinderApprovedStandardDetailsHandler(_mockLogger.Object, _mockOppFinderRepository.Object, _mockMediator.Object);
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().NotBeNull();
            result.EqaProvider.Should().Be(eqaProvider);
        }
    }
}
