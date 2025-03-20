using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.OppFinder
{
    public class WhenHandlingGetOppFinderApprovedStandardDetailsRequest
    {
        private Mock<ILogger<GetOppFinderApprovedStandardDetailsHandler>> _mockLogger;
        private Mock<IOppFinderRepository> _mockOppFinderRepository;
        private Mock<IMediator> _mockMediator;

        private GetOppFinderApprovedStandardDetailsHandler _sut;
        private string[] validationRequestValidEmailValues = { "", null, "valid@email.com" };


        [SetUp]
        public void Setup()
        {
            _mockOppFinderRepository = new Mock<IOppFinderRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<GetOppFinderApprovedStandardDetailsHandler>>();
            
            _mockMediator.Setup(s => s.Send(It.Is<ValidationRequest>(x => x.Type == "email" && validationRequestValidEmailValues.Contains(x.Value)), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

            _mockMediator.Setup(s => s.Send(It.Is<ValidationRequest>(x => x.Type == "email" && x.Value == "invalid"), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    ApprovedForDelivery = new DateTime(2021, 12, 15)
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
                result.ApprovedForDelivery.Should().Be("15 December 2021");
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
        public async Task ThenReturnsEqaProviderContactEmailInEqaProvider()
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };


            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactEmail = "valid@email.com",
                },
            });



            //Act

            _sut = new GetOppFinderApprovedStandardDetailsHandler(_mockLogger.Object, _mockOppFinderRepository.Object, _mockMediator.Object);
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.EqaProvider.Should().Be("valid@email.com");
            }

        }

        [Test]
        [TestCase("", "valid@email.com")]
        [TestCase(null, "valid@email.com")]
        [TestCase("invalid", "valid@email.com")]
        public async Task ThenReturnsEqaProviderContactNameInEqaProvider(string eqaProviderContactEmail, string eqaProviderContactName)
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactName = eqaProviderContactName,
                    EqaProviderContactEmail = eqaProviderContactEmail
                },
            });


            //Act

            _sut = new GetOppFinderApprovedStandardDetailsHandler(_mockLogger.Object, _mockOppFinderRepository.Object, _mockMediator.Object);
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.EqaProvider.Should().Be(eqaProviderContactName);
            }

        }

        [Test]
        [TestCase("", "", "OFQUAL")]
        [TestCase(null, null, "OFQUAL")]
        [TestCase("invalid", "invalid", "OFQUAL")]
        public async Task ThenReturnsEqaProviderNameInEqaProvider(string eqaProviderContactEmail, string eqaProviderContactName, string eqaProviderName)
        {
            //Arrange
            var request = new GetOppFinderApprovedStandardDetailsRequest()
            {
                StandardReference = "ST0001"
            };

            _mockOppFinderRepository.Setup(s => s.GetOppFinderApprovedStandardDetails("ST0001")).ReturnsAsync(new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = new OppFinderApprovedStandardOverviewResult()
                {
                    StandardCode = 999,
                    StandardReference = "ST0001",
                    EqaProviderContactName = eqaProviderContactName,
                    EqaProviderContactEmail = eqaProviderContactEmail,
                    EqaProviderName = eqaProviderName
                },
            });


            //Act

            _sut = new GetOppFinderApprovedStandardDetailsHandler(_mockLogger.Object, _mockOppFinderRepository.Object, _mockMediator.Object);
            var result = await _sut.Handle(request, new CancellationToken());

            //Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.EqaProvider.Should().Be(eqaProviderName);
            }

        }
    }
}
