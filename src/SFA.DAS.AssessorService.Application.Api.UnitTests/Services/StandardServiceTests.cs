using AutoFixture.NUnit3;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.ExternalApis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    public class StandardServiceTests
    {
        private Mock<IOuterApiClient> _mockOuterApiClient;
        private Mock<ILogger<StandardService>> _mockLogger;
        private StandardService _standardService;

        [SetUp]
        public void Setup()
        {
            _mockOuterApiClient = new Mock<IOuterApiClient>();
            _mockLogger = new Mock<ILogger<StandardService>>();

            _standardService = new StandardService(new CacheService(Mock.Of<IDistributedCache>()),
                _mockOuterApiClient.Object,
                Mock.Of<IIfaStandardsApiClient>(),
                _mockLogger.Object,
                Mock.Of<IStandardRepository>());
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptions_And_OuterApiReturnsStandardOptionsListResponse_Then_ReturnsListOfStandardOptions(GetStandardOptionsListResponse response)
        {
            _mockOuterApiClient.Setup(client => client.Get<GetStandardOptionsListResponse>(It.IsAny<GetStandardOptionsRequest>()))
                .ReturnsAsync(response);

            var result = await _standardService.GetStandardOptions();

            Assert.IsInstanceOf<IEnumerable<StandardOptions>>(result);
            Assert.AreEqual(result.Count(), response.StandardOptions.Count());
        }

        [Test]
        public async Task When_GettingStandardOptions_And_OuterApiDoesNotReturnResponse_Then_LogError_And_ReturnNull()
        {
            _mockOuterApiClient.Setup(client => client.Get<GetStandardOptionsListResponse>(It.IsAny<GetStandardOptionsRequest>()))
                .ReturnsAsync((GetStandardOptionsListResponse)null);

            var result = await _standardService.GetStandardOptions();

            Assert.IsNull(result);

            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), 
                    It.IsAny<FormattedLogValues>(),
                    It.IsAny<Exception>(), 
                    It.IsAny<Func<object, Exception, string>>()), 
                Times.Once, "STANDARD OPTIONS: Failed to get standard options");
        }
    }
}
