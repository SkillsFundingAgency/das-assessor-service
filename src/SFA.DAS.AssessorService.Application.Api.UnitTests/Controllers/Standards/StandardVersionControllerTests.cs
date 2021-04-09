using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards
{
    public class StandardVersionControllerTests
    {
        private Mock<IStandardService> _mockStandardService;

        private StandardVersionController _standardVersionController;

        [SetUp]
        public void SetUp()
        {
            _mockStandardService = new Mock<IStandardService>();

            _standardVersionController = new StandardVersionController(Mock.Of<ILogger<StandardServiceController>>(), _mockStandardService.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetAllStandardVersions_ThenListOfStandardVersionsIsReturned(IEnumerable<Standard> standards)
        {
            _mockStandardService.Setup(s => s.GetAllStandardVersions()).ReturnsAsync(standards);

            var controllerResult = await _standardVersionController.GetAllStandardVersions() as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<StandardVersion>;

            var expectedResponse = standards.Select(ConvertFromStandard);

            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionsByLarsCode_ThenListOfStandardVersionsByLarsCodeIsReturned(int larsCode, IEnumerable<Standard> standards)
        {
            _mockStandardService.Setup(s => s.GetStandardVersionsByLarsCode(larsCode)).ReturnsAsync(standards);

            var controllerResult = await _standardVersionController.GetStandardVersionsByLarsCode(larsCode) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as IEnumerable<StandardVersion>;

            var expectedResponse = standards.Select(ConvertFromStandard);
            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionByStandardUId_ThenAStandardVersionsIsReturned(string standardUId, Standard standard)
        {
            _mockStandardService.Setup(s => s.GetStandardVersionByStandardUId(standardUId)).ReturnsAsync(standard);

            var controllerResult = await _standardVersionController.GetStandardVersionByStandardUId(standardUId) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as StandardVersion;

            var expectedResponse = ConvertFromStandard(standard);
            model.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardVersionByStandardUId_ButTheStandardCannotBeFound(string standardUId)
        {
            Standard result = null;
            _mockStandardService.Setup(s => s.GetStandardVersionByStandardUId(standardUId)).ReturnsAsync(result);

            var controllerResult = await _standardVersionController.GetStandardVersionByStandardUId(standardUId) as NotFoundResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        private StandardVersion ConvertFromStandard(Standard standard)
        {
            return new StandardVersion
            {
                EffectiveFrom = standard.EffectiveFrom.GetValueOrDefault(),
                IFateReferenceNumber = standard.IfateReferenceNumber,
                LarsCode = standard.LarsCode,
                Level = standard.Level,
                StandardUId = standard.StandardUId,
                Title = standard.Title,
                Version = standard.Version.ToString()
            };
        }
    }
}
