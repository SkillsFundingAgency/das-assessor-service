using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Standards
{
    public class StandardServiceControllerTests
    {
        private Mock<IStandardService> _mockStandardService;

        private StandardServiceController _standardServiceController;

        [SetUp]
        public void SetUp()
        {
            _mockStandardService = new Mock<IStandardService>();

            _standardServiceController = new StandardServiceController(Mock.Of<ILogger<StandardServiceController>>(), _mockStandardService.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingGetStandardOptions_ThenListOfStandardsWithTheirOptionsIsReturned(IEnumerable<StandardOptions> standardOptions)

        {
            _mockStandardService.Setup(service => service.GetStandardOptions())
                .ReturnsAsync(standardOptions);

            var controllerResult = await _standardServiceController.GetStandardOptions() as ObjectResult;

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

            var controllerResult = await _standardServiceController.GetStandardOptionsForStandard(standard) as ObjectResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = controllerResult.Value as StandardOptions;

            model.Should().BeEquivalentTo(options);
        }
    }
}
