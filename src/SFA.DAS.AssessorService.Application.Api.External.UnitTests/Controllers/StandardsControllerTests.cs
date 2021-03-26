using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.External.Controllers;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.UnitTests.Controllers
{
    public class StandardsControllerTests
    {
        private Fixture _fixture;

        private Mock<IApiClient> _mockApiClient;

        private StandardsController _controller;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _mockApiClient = new Mock<IApiClient>();

            _controller = new StandardsController(Mock.Of<ILogger<StandardsController>>(), Mock.Of<IHeaderInfo>(), _mockApiClient.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingStandardVersionOptions_And_StandardVersionWithOptionsIsFound_Then_ReturnStandard(string standardReference, string version, StandardOptions standardOptionsResponse)
        {
            _mockApiClient.Setup(client => client.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version))
                .ReturnsAsync(standardOptionsResponse);

            var result = await _controller.GetOptionsForStandardVersion(standardReference, version) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var model = result.Value as StandardOptions;

            model.Should().BeEquivalentTo(standardOptionsResponse);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingStandardVersionOptions_And_StandardVersionWithNoOptionsIsFound_Then_ReturnNoContent(string standardReference, string version)
        {
            var standardOptionsResponse = _fixture.Build<StandardOptions>()
                .With(s => s.CourseOption, new List<string>())
                .Create();

            _mockApiClient.Setup(client => client.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version))
                .ReturnsAsync(standardOptionsResponse);

            var result = await _controller.GetOptionsForStandardVersion(standardReference, version) as NoContentResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingStandardVersionOptions_And_StandardVersionIsNotFound_Then_ReturnNotFound(string standardReference, string version)
        {
            _mockApiClient.Setup(client => client.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version))
                .ReturnsAsync((StandardOptions)null);

            var result = await _controller.GetOptionsForStandardVersion(standardReference, version) as NotFoundResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

    }
}
