using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards
{
    public class StandardVersionControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<IStandardService> _mockStandardService;

        private StandardVersionController _standardVersionController;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mockStandardService = new Mock<IStandardService>();

            _standardVersionController = new StandardVersionController(_mockMediator.Object, _backgroundTaskQueue.Object, Mock.Of<ILogger<StandardVersionController>>(), _mockStandardService.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetAllStandardVersions_ThenListOfStandardVersionsIsReturned(IEnumerable<Standard> standards)
        {
            _mockStandardService.Setup(s => s.GetAllStandardVersions()).ReturnsAsync(standards);

            var controllerResult = await _standardVersionController.GetAllStandardVersions() as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<StandardVersion>;

            var expectedResponse = standards.Select(s => (StandardVersion)s);

            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionsByLarsCode_ThenListOfStandardVersionsByLarsCodeIsReturned(int larsCode, IEnumerable<Standard> standards)
        {
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(larsCode)).ReturnsAsync(standards);

            var controllerResult = await _standardVersionController.GetStandardVersionsByLarsCode(larsCode) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<StandardVersion>;

            var expectedResponse = standards.Select(s => (StandardVersion)s);
            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionByStandardUId_ThenAStandardVersionsIsReturned(string standardUId, Standard standard)
        {
            _mockStandardService.Setup(s => s.GetStandardVersionById(standardUId, null)).ReturnsAsync(standard);

            var controllerResult = await _standardVersionController.GetStandardVersionById(standardUId) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as StandardVersion;

            StandardVersion expectedResponse = standard;
            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionByStandardUId_ButTheStandardCannotBeFound(string standardUId)
        {
            Standard result = null;
            _mockStandardService.Setup(s => s.GetStandardVersionById(standardUId, null)).ReturnsAsync(result);

            var controllerResult = await _standardVersionController.GetStandardVersionById(standardUId) as NotFoundResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetEPAORegisteredStandardVersions_ReturnsListOfApprovedStandardVersions(string epaoId, IEnumerable<OrganisationStandardVersion> versions)
        {
            _mockStandardService.Setup(s => s.GetEPAORegisteredStandardVersions(epaoId, null)).ReturnsAsync(versions);

            var controllerResult = await _standardVersionController.GetEpaoRegisteredStandardVersions(epaoId) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<OrganisationStandardVersion>;

            model.Should().BeEquivalentTo(versions);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetEPAORegisteredStandardVersionsWithLarsCode_ReturnsListOfApprovedStandardVersionsForThatLarsCode(string epaoId, int larsCode, IEnumerable<OrganisationStandardVersion> versions)
        {
            _mockStandardService.Setup(s => s.GetEPAORegisteredStandardVersions(epaoId, larsCode)).ReturnsAsync(versions);

            var controllerResult = await _standardVersionController.GetEpaoRegisteredStandardVersionsByLarsCode(epaoId, larsCode) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<OrganisationStandardVersion>;

            model.Should().BeEquivalentTo(versions);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardOptions_ThenListOfStandardsWithTheirOptionsIsReturned(IEnumerable<StandardOptions> standardOptions)

        {
            _mockStandardService.Setup(service => service.GetAllStandardOptions())
                .ReturnsAsync(standardOptions);

            var controllerResult = await _standardVersionController.GetStandardOptions() as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<StandardOptions>;

            var expectedResponse = standardOptions.Select(s => new StandardOptions
            {
                StandardUId = s.StandardUId,
                StandardCode = s.StandardCode,
                StandardReference = s.StandardReference,
                Version = s.Version,
                CourseOption = s.CourseOption
            });

            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardOptionByStandardId_ThenListOfOptionsForThatStandardIsReturned(StandardOptions options, string standard)

        {
            _mockStandardService.Setup(service => service.GetStandardOptionsByStandardId(standard))
                .ReturnsAsync(options);

            var controllerResult = await _standardVersionController.GetStandardOptionsForStandard(standard) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as StandardOptions;

            model.Should().BeEquivalentTo(options);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetLatestStandardVersions_ThenLatestStandardVersionsAreReturned(List<Standard> standards)
        {
            _mockStandardService.Setup(service => service.GetLatestStandardVersions()).ReturnsAsync(standards);

            var controllerResult = await _standardVersionController.GetLatestStandardVersions() as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as List<StandardVersion>;

            model.Count.Should().Be(standards.Count);
            model[0].IFateReferenceNumber.Should().Be(standards[0].IfateReferenceNumber);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetEpaoRegisteredStandardVersions_ThenStandardVersionsAreReturned(string epaoId, string iFateReferenceNumber, List<StandardVersion> versions)
        {
            _mockStandardService.Setup(service => service.GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(epaoId, iFateReferenceNumber)).ReturnsAsync(versions);

            var controllerResult = await _standardVersionController.GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(epaoId, iFateReferenceNumber) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as List<StandardVersion>;

            model.Should().BeEquivalentTo(versions);
        }
    }
}
