using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Mapping.Structs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    public class StandardServiceTests
    {
        private Mock<ILogger<StandardService>> _mockLogger;
        private Mock<IStandardRepository> _mockStandardRepository;

        private StandardService _standardService;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<StandardService>>();
            _mockStandardRepository = new Mock<IStandardRepository>();

            _standardService = new StandardService(new CacheService(Mock.Of<IDistributedCache>()),
                _mockLogger.Object,
                _mockStandardRepository.Object);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptions_Then_ReturnsListOfStandardOptions(IEnumerable<StandardOptions> options)
        {
            _mockStandardRepository.Setup(s => s.GetAllStandardOptions()).ReturnsAsync(options);

            var result = await _standardService.GetAllStandardOptions();

            Assert.IsInstanceOf<IEnumerable<StandardOptions>>(result);
            Assert.AreEqual(result.Count(), options.Count());
        }

        [Test]
        public async Task When_GettingStandardOptions_And_ExceptionIsThrown_Then_LogError_And_ReturnNull()
        {
            _mockStandardRepository.Setup(s => s.GetAllStandardOptions()).Throws<TimeoutException>();

            var result = await _standardService.GetAllStandardOptions();

            Assert.IsNull(result);
            VerifyLogger(LogLevel.Error, new EventId(0), "STANDARD OPTIONS: Failed to get standard options");
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsForLatestVersion_Then_ReturnsListOfStandardOptions(IEnumerable<StandardOptions> standardOptions)
        {
            _mockStandardRepository.Setup(s => s.GetStandardOptionsForLatestStandardVersions()).ReturnsAsync(standardOptions);

            var result = await _standardService.GetStandardOptionsForLatestStandardVersions();

            Assert.IsInstanceOf<IEnumerable<StandardOptions>>(result);
            Assert.AreEqual(result.Count(), standardOptions.Count());
        }

        [Test]
        public async Task When_GettingStandardOptionsForLatestVersionThrowsException_Then_LogError_And_ReturnNull()
        {
            _mockStandardRepository.Setup(s => s.GetStandardOptionsForLatestStandardVersions()).Throws<TimeoutException>();

            var result = await _standardService.GetStandardOptionsForLatestStandardVersions();

            Assert.IsNull(result);
            VerifyLogger(LogLevel.Error, new EventId(0), "STANDARD OPTIONS: Failed to get options for latest version of each standard");
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_AndIdIsStandardUId_Then_ReturnsStandardOptions(StandardOptions option)
        {
            var standardUId = "ST0023_1.0";
            _mockStandardRepository.Setup(s => s.GetStandardOptionsByStandardUId(standardUId)).ReturnsAsync(option);

            var result = await _standardService.GetStandardOptionsByStandardId(standardUId);

            result.Should().BeEquivalentTo(option);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_AndIdIsLarsCode_Then_ReturnsStandardOptions(StandardOptions option, int larsCode)
        {
            _mockStandardRepository.Setup(s => s.GetStandardOptionsByLarsCode(larsCode)).ReturnsAsync(option);

            var result = await _standardService.GetStandardOptionsByStandardId(larsCode.ToString());

            result.Should().BeEquivalentTo(option);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_AndIdIsStandardReferenceNumber_Then_ReturnsStandardOptions(StandardOptions option)
        {
            var ifateReferenceNumber = "ST0023";
            _mockStandardRepository.Setup(s => s.GetStandardOptionsByIFateReferenceNumber(ifateReferenceNumber)).ReturnsAsync(option);

            var result = await _standardService.GetStandardOptionsByStandardId(ifateReferenceNumber);

            result.Should().BeEquivalentTo(option);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardId_AndAnExceptionIsThrown_Then_LogError_And_ReturnNull()
        {
            var standardUId = "ST0023_1.0";
            _mockStandardRepository.Setup(s => s.GetStandardOptionsByStandardUId(standardUId)).Throws<TimeoutException>();

            var result = await _standardService.GetStandardOptionsByStandardId(standardUId);
            Assert.IsNull(result);
            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD OPTIONS: Failed to get standard options for id {standardUId}");
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardIdAndVersion_And_IdIsIfateReferenceNumber_Then_StandardIsRetrievedFromAssessorStandardsTable(string version, Standard getStandardResponse)
        {
            var id = "ST0001";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(id, version))
                .ReturnsAsync(getStandardResponse);

            await _standardService.GetStandardOptionsByStandardIdAndVersion(id, version);

            _mockStandardRepository.Verify(repository => repository.GetStandardVersionByIFateReferenceNumber(id, version), Times.Once);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardIdAndVersion_And_IdIsLarsCode_Then_StandardIsRetrievedFromAssessorStandardsTable(string version, Standard getStandardResponse)
        {
            var id = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(int.Parse(id), version))
                .ReturnsAsync(getStandardResponse);

            await _standardService.GetStandardOptionsByStandardIdAndVersion(id, version);

            _mockStandardRepository.Verify(repository => repository.GetStandardVersionByLarsCode(int.Parse(id), version), Times.Once);
        }
        
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_Then_UseStandardUIdToCallOuterApi(string version, Standard getStandardResponse, StandardOptions option)
        {
            var standardReference = "ST0001";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(standardReference, version))
                .ReturnsAsync(getStandardResponse);

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByStandardUId(getStandardResponse.StandardUId)).ReturnsAsync(option);

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardReference, version);

            result.Should().BeEquivalentTo(option);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByLarsCodeAndVersion_Then_UseStandardUIdToCallOuterApi(string version, Standard getStandardResponse, StandardOptions option)
        {
            var standardId = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(int.Parse(standardId), version))
                .ReturnsAsync(getStandardResponse);

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByStandardUId(getStandardResponse.StandardUId)).ReturnsAsync(option);

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardId, version);

            result.Should().BeEquivalentTo(option);
        }

        [Test, AutoData]
        public async Task When_GettingAllStandardVersions_Then_ReturnsListOfStandards(IEnumerable<Standard> standards)
        {
            _mockStandardRepository.Setup(s => s.GetAllStandards()).ReturnsAsync(standards);

            var result = await _standardService.GetAllStandardVersions();

            Assert.IsInstanceOf<IEnumerable<Standard>>(result);
            Assert.AreEqual(result.Count(), standards.Count());
        }

        [Test, AutoData]
        public async Task When_GettingAllStandardVersions_OfAGivenStandardId_Then_ReturnsAListOfThatStandardsVersions(int standardId, IEnumerable<Standard> standards)
        {
            _mockStandardRepository.Setup(s => s.GetStandardVersionsByLarsCode(standardId)).ReturnsAsync(standards);

            var result = await _standardService.GetStandardVersionsByLarsCode(standardId);

            Assert.IsInstanceOf<IEnumerable<Standard>>(result);
            Assert.AreEqual(result.Count(), standards.Count());
        }

        [Test, AutoData]
        public async Task When_GettingAStandardVersion_ByStandardUId(string standardUId, Standard standard)
        {
            _mockStandardRepository.Setup(s => s.GetStandardVersionByStandardUId(standardUId)).ReturnsAsync(standard);

            var result = await _standardService.GetStandardVersionByStandardUId(standardUId);

            result.Should().BeEquivalentTo(standard);
        }
        public async Task When_GettingAStandardVersion_ByLarsCode(int larsCode, string version, Standard standard)
        {
            _mockStandardRepository.Setup(s => s.GetStandardVersionByLarsCode(larsCode, version)).ReturnsAsync(standard);

            var result = await _standardService.GetStandardVersionById(larsCode.ToString());

            result.Should().BeEquivalentTo(standard);
        }

        public async Task When_GettingAStandardVersion_IFateReferenceNumber(string ifateReferenceNumber, string version, Standard standard)
        {
            _mockStandardRepository.Setup(s => s.GetStandardVersionByIFateReferenceNumber(ifateReferenceNumber, version)).ReturnsAsync(standard);

            var result = await _standardService.GetStandardVersionById(ifateReferenceNumber);

            result.Should().BeEquivalentTo(standard);
        }

        [Test, RecursiveMoqAutoData]
        public async Task When_GettingEpaoRegisteredStandardVersionsByEpaoId_ReturnsApprovedStandardVersions(string epaoId, IEnumerable<OrganisationStandardVersion> standards)
        {
            _mockStandardRepository.Setup(s => s.GetEpaoRegisteredStandardVersions(epaoId)).ReturnsAsync(standards);

            var result = await _standardService.GetEPAORegisteredStandardVersions(epaoId, null);

            result.Should().BeEquivalentTo(standards.Select(s => new { StandardUId = s.StandardUId, Version = s.Version }));
        }

        [Test, RecursiveMoqAutoData]
        public async Task When_GettingEpaoRegisteredStandardVersionsByEpaoIdAndLarsCode_ReturnsApprovedStandardVersionsForThatStandard(string epaoId, int larsCode, IEnumerable<OrganisationStandardVersion> standards)
        {
            _mockStandardRepository.Setup(s => s.GetEpaoRegisteredStandardVersions(epaoId, larsCode)).ReturnsAsync(standards);

            var result = await _standardService.GetEPAORegisteredStandardVersions(epaoId, larsCode);

            result.Should().BeEquivalentTo(standards.Select(s => new { StandardUId = s.StandardUId, Version = s.Version }));
        }

        [Test, AutoData]
        public async Task When_GetLatestStandardVersions_ReturnsLatestStandards(IEnumerable<Standard> standards)
        {
            _mockStandardRepository.Setup(s => s.GetLatestStandardVersions()).ReturnsAsync(standards);

            var result = await _standardService.GetLatestStandardVersions();

            result.Should().BeEquivalentTo(standards);
        }

        [Test, AutoData]
        public async Task When_GetEPAORegisteredStandardVersions_Returns_The_Orgs_Standards(string endPointAssessorOrganisationId, string iFateReferenceNumber, IEnumerable<StandardVersion> versions)
        {
            _mockStandardRepository.Setup(s => s.GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(endPointAssessorOrganisationId, iFateReferenceNumber)).ReturnsAsync(versions);

            var result = await _standardService.GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(endPointAssessorOrganisationId, iFateReferenceNumber);

            result.Should().BeEquivalentTo(versions);
        }


        [Test, AutoData]
        public async Task When_GettingStandardVersionsByIFateReferenceNumber_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<Standard>)null);

            var result = await _standardService.GetStandardVersionsByIFateReferenceNumber(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for IFATE reference number: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionsByIFateReferenceNumber_And_ExceptionThrown_Then_LogError_And_ReturnNull(string standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionsByIFateReferenceNumber(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionsByIFateReferenceNumber(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD VERSION: Failed to get for iFateReferenceNumber: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_TheStandardIdTypeIsLarsCode_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for Id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.LarsCode}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_TheStandardIdTypeIsIFateReferenceNumber_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for Id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.IFateReferenceNumber}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_TheStandardIdTypeIsStandardUId_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = "TEST";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByStandardUId(It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for Id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.StandardUId}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_TheStandardIdTypeUnknown_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = null;

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for Id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.Unknown}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_StandardIdTypeIsLarsCode_ExceptionThrown_Then_LogError_And_ReturnNull(string version)
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD VERSION: Failed to get for standard id: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_StandardIdTypeIsIFateReferenceNumber_ExceptionThrown_Then_LogError_And_ReturnNull(string version)
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD VERSION: Failed to get for standard id: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionById_And_StandardIdTypeIsStandardUId_ExceptionThrown_Then_LogError_And_ReturnNull(string version)
        {
            string standardReference = "TEST";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByStandardUId(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionById(standardReference, version);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD VERSION: Failed to get for standard id: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionsByLarsCode_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(int standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionsByLarsCode(It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<Standard>)null);

            var result = await _standardService.GetStandardVersionsByLarsCode(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for Id: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardVersionsByLarsCode_And_ThrowsException_Then_LogError_And_ReturnNull(int standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionsByLarsCode(It.IsAny<int>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionsByLarsCode(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD: Failed to get for LARS code: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingGetStandardVersionByStandardUId_And_StandardIsNotFound_Then_LogInfo_And_ReturnNull(string standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByStandardUId(It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardVersionByStandardUId(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard for standard uid: {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingGetStandardVersionByStandardUId_And_ThrowsException_Then_LogError_And_ReturnNull(string standardReference)
        {
            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByStandardUId(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardVersionByStandardUId(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD: Failed to get for standard id: {standardReference}");

            Assert.AreEqual(null, result);
        }
        
        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsLarsCode_StandardOptionsIsNotFound_Then_LogInfo_And_ReturnNull()
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByLarsCode(It.IsAny<int>()))
                .ReturnsAsync((StandardOptions)null);

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard options for id: {standardReference} and standard id type: {StandardId.StandardIdType.LarsCode}");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsIFateReferenceNumber_StandardOptionsIsNotFound_Then_LogInfo_And_ReturnNull()
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByIFateReferenceNumber(It.IsAny<string>()))
                .ReturnsAsync((StandardOptions)null);

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard options for id: {standardReference} and standard id type: {StandardId.StandardIdType.IFateReferenceNumber}");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsStandardUId_StandardOptionsIsNotFound_Then_LogInfo_And_ReturnNull()
        {
            string standardReference = "TEST";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByStandardUId(It.IsAny<string>()))
                .ReturnsAsync((StandardOptions)null);

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard options for id: {standardReference} and standard id type: {StandardId.StandardIdType.StandardUId}");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsLarsCode_ThrowsException_Then_LogError_And_ReturnNull()
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByLarsCode(It.IsAny<int>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD OPTIONS: Failed to get standard options for id {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsIFateReferenceNumber_ThrowsException_Then_LogError_And_ReturnNull()
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByIFateReferenceNumber(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD OPTIONS: Failed to get standard options for id {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task When_GettingStandardOptionsByStandardId_And_StandardIdTypeIsStandardUId_ThrowsException_Then_LogError_And_ReturnNull()
        {
            string standardReference = "TEST";

            _mockStandardRepository.Setup(repository => repository.GetStandardOptionsByStandardUId(It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardId(standardReference);

            VerifyLogger(LogLevel.Error, new EventId(0), $"STANDARD OPTIONS: Failed to get standard options for id {standardReference}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]        
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_And_StandardIdTypeIsIFateReferenceNumber_And_StandardWithReferenceAndVersionIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard options for id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.IFateReferenceNumber}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_And_StandardIdTypeIsLarsCode_And_StandardWithReferenceAndVersionIsNotFound_Then_LogInfo_And_ReturnNull(string version)
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((Standard)null);

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardReference, version);

            VerifyLogger(LogLevel.Information, new EventId(0), $"Unable to find standard options for id: {standardReference}, version: {version} and standard id type: {StandardId.StandardIdType.LarsCode}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_And_StandardIdTypeIsIFateReferenceNumber_And_ThrowsException_Then_LogError_And_ReturnNull(string version)
        {
            string standardReference = "IFATE1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByIFateReferenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardReference, version);

            VerifyLogger(LogLevel.Error, new EventId(0), $"Could not find standard with id: {standardReference} and Version: {version}");

            Assert.AreEqual(null, result);
        }

        [Test, AutoData]
        public async Task When_GettingStandardOptionsByStandardReferenceAndVersion_And_StandardIdTypeIsLarsCode_And_ThrowsException_Then_LogError_And_ReturnNull(string version)
        {
            string standardReference = "1";

            _mockStandardRepository.Setup(repository => repository.GetStandardVersionByLarsCode(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());

            var result = await _standardService.GetStandardOptionsByStandardIdAndVersion(standardReference, version);

            VerifyLogger(LogLevel.Error, new EventId(0), $"Could not find standard with id: {standardReference} and Version: {version}");

            Assert.AreEqual(null, result);
        }

        private void VerifyLogger(LogLevel logLevel, EventId eventId, string message)
        {
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(p => p == logLevel),
                It.Is<EventId>(p => p == eventId),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == message && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
