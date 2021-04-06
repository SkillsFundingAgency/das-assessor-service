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
using SFA.DAS.AssessorService.Domain.Entities;
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
        private Mock<IStandardRepository> _mockStandardRepository;

        private StandardService _standardService;

        [SetUp]
        public void Setup()
        {
            _mockOuterApiClient = new Mock<IOuterApiClient>();
            _mockLogger = new Mock<ILogger<StandardService>>();
            _mockStandardRepository = new Mock<IStandardRepository>();

            _standardService = new StandardService(new CacheService(Mock.Of<IDistributedCache>()),
                _mockOuterApiClient.Object,
                _mockLogger.Object,
                _mockStandardRepository.Object);
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

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_And_OuterApiReturnsStandardDetailResponse_Then_ReturnsStandardOptions(StandardDetailResponse response, string id)
        {
            _mockOuterApiClient.Setup(client => client.Get<StandardDetailResponse>(It.Is<GetStandardByIdRequest>(x => x.Id == id)))
                .ReturnsAsync(response);

            var result = await _standardService.GetStandardOptionsByStandardId(id);

            Assert.IsInstanceOf<StandardOptions>(result);
            Assert.AreEqual(result.CourseOption, response.Options);
            Assert.AreEqual(result.StandardUId, response.StandardUId);
            Assert.AreEqual(result.StandardCode, response.LarsCode);
            Assert.AreEqual(result.StandardReference, response.IfateReferenceNumber);
            Assert.AreEqual(result.Version, response.Version.ToString("#.0"));
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_And_OuterApiDoesNotReturnResponse_Then_LogError_And_ReturnNull(string id)
        {
            _mockOuterApiClient.Setup(client => client.Get<StandardDetailResponse>(It.Is<GetStandardByIdRequest>(x => x.Id == id)))
                .ReturnsAsync((StandardDetailResponse)null);

            var result = await _standardService.GetStandardOptionsByStandardId(id);
            Assert.IsNull(result);

            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.IsAny<FormattedLogValues>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()),
                Times.Once, $"STANDARD OPTIONS: Failed to get standard options for id {id}");
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_Then_StandardIsRetrievedFromAssessorStandardsTable(string standardReference, string version, Standard getStandardResponse)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardByStandardReferenceAndVersion(standardReference, version))
                .ReturnsAsync(getStandardResponse);

            await _standardService.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version);

            _mockStandardRepository.Verify(repository => repository.GetStandardByStandardReferenceAndVersion(standardReference, version), Times.Once);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_And_StandardWithReferenceAndVersionIsNotFound_Then_LogError_And_ReturnNull(string standardReference, string version)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardByStandardReferenceAndVersion(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version);
            
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(),
                    It.IsAny<FormattedLogValues>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()),
                Times.Once, $"Could not find standard with StandardReference: { standardReference } and Version: { version}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_Then_UseStandardUIdToCallOuterApi(string standardReference, string version, Standard getStandardResponse, StandardDetailResponse StandardDetailResponse)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardByStandardReferenceAndVersion(standardReference, version))
                .ReturnsAsync(getStandardResponse);

            _mockOuterApiClient.Setup(client => client.Get<StandardDetailResponse>(It.Is<GetStandardByIdRequest>(x => x.Id == getStandardResponse.StandardUId)))
                .ReturnsAsync(StandardDetailResponse);

            var result = await _standardService.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version);

            Assert.IsInstanceOf<StandardOptions>(result);
            _mockOuterApiClient.Verify(client => client.Get<StandardDetailResponse>(It.Is<GetStandardByIdRequest>(x => x.Id == getStandardResponse.StandardUId)), Times.Once);
        }

        [Test, AutoData]
        public async Task When_GettingAllStandardVersions_Then_ReturnsListOfStandards(IEnumerable<Standard> standards)
        {
            _mockStandardRepository.Setup(s => s.GetAllStandards()).ReturnsAsync(standards);

            var result = await _standardService.GetAllStandardVersions();

            Assert.IsInstanceOf<IEnumerable<Standard>>(result);
            Assert.AreEqual(result.Count(), standards.Count());
        }
    }
}
