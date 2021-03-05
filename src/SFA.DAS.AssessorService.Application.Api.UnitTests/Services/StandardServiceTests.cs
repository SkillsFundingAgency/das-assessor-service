using AutoFixture.NUnit3;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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

        private StandardService _standardService;

        [SetUp]
        public void Setup()
        {
            _mockOuterApiClient = new Mock<IOuterApiClient>();

            _standardService = new StandardService(new CacheService(Mock.Of<IDistributedCache>()),
                _mockOuterApiClient.Object,
                Mock.Of<IIfaStandardsApiClient>(),
                Mock.Of<ILogger<StandardService>>(),
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
    }
}
