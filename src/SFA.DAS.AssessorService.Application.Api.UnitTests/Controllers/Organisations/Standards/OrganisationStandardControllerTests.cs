using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Standards
{
    public class OrganisationStandardControllerTests
    {
        private Mock<ILogger<OrganisationStandardController>> _mockLogger;
        private Mock<IMediator> _mockMediator;

        private OrganisationStandardController _controller;
       

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<OrganisationStandardController>>();
            _mockMediator = new Mock<IMediator>();

            _controller = new OrganisationStandardController(_mockLogger.Object, _mockMediator.Object);
        }

        [Test]
        public async Task OptInOrganisationStandardVersion_ReturnsCreatedAtRouteResultWithStandardUIdAndVersion_WhenPostCalled()
        {
            var request = new OrganisationStandardVersionOptInRequest();

            _mockMediator.Setup(s => s.Send(request, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var controllerResult = await _controller.OptInOrganisationStandardVersion(request) as CreatedAtRouteResult;

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var model = controllerResult.Value as OrganisationStandardVersion;

            model.Version.Should().Be("1.2");
            model.StandardUId.Should().Be("ST0001_1.2");
        }
    }
}
