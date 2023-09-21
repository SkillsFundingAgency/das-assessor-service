using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Standards
{
    [TestFixture]
    public class OrganisationStandardControllerTests
    {
        private Mock<ILogger<OrganisationStandardController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private OrganisationStandardController _controller;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrganisationStandardController>>();
            _mediatorMock = new Mock<IMediator>();
            _controller = new OrganisationStandardController(_loggerMock.Object, _mediatorMock.Object);
        }

        [Test]
        public async Task OptInOrganisationStandardVersion_ReturnsCreatedResult_OnSuccess()
        {
            var request = new OrganisationStandardVersionOptInRequest();
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.OptInOrganisationStandardVersion(request);

            Assert.IsInstanceOf<CreatedAtRouteResult>(result);
        }

        [Test]
        public async Task OptOutOrganisationStandardVersion_ReturnsOkResult_OnSuccess()
        {
            var request = new OrganisationStandardVersionOptOutRequest();
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.OptOutOrganisationStandardVersion(request);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateOrganisationStandardVersion_ReturnsOkResult_OnSuccess()
        {
            var request = new UpdateOrganisationStandardVersionRequest();
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.UpdateOrganisationStandardVersion(request);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task OptInOrganisationStandardVersion_ReturnsCorrectData_OnSuccess()
        {
            var request = new OrganisationStandardVersionOptInRequest();

            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.OptInOrganisationStandardVersion(request) as CreatedAtRouteResult;
            var model = result.Value as OrganisationStandardVersion;

            Assert.That(result, Is.Not.Null);
            Assert.That(model.Version, Is.EqualTo("1.2"));
            Assert.That(model.StandardUId, Is.EqualTo("ST0001_1.2"));
        }

        [Test]
        public async Task OptInOrganisationStandardVersion_ReturnsBadRequest_OnError()
        {
            var request = new OrganisationStandardVersionOptInRequest();

            var exMessage = "Test Exception Message";
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Throws(new Exception(exMessage));

            var result = await _controller.OptInOrganisationStandardVersion(request) as BadRequestObjectResult;
            var response = result.Value as EpaoStandardVersionResponse;

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Details, Is.EqualTo(exMessage));
        }

        [Test]
        public async Task OptOutOrganisationStandardVersion_ReturnsCorrectData_OnSuccess()
        {
            var request = new OrganisationStandardVersionOptOutRequest();
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.OptOutOrganisationStandardVersion(request) as OkObjectResult;

            var model = result.Value as EpaoStandardVersionResponse;

            Assert.That(result, Is.Not.Null);
            Assert.That(model.Details, Is.EqualTo("1.2"));
        }

        [Test]
        public async Task OptOutOrganisationStandardVersion_ReturnsBadRequest_OnError()
        {
            var request = new OrganisationStandardVersionOptOutRequest();
            var exMessage = "Test Exception Message";
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Throws(new Exception(exMessage));

            var result = await _controller.OptOutOrganisationStandardVersion(request) as BadRequestObjectResult;
            var response = result.Value as EpaoStandardVersionResponse;

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Details, Is.EqualTo(exMessage));
        }

        [Test]
        public async Task UpdateOrganisationStandardVersion_ReturnsCorrectData_OnSuccess()
        {
            var request = new UpdateOrganisationStandardVersionRequest();
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new OrganisationStandardVersion() { Version = "1.2", StandardUId = "ST0001_1.2" }));

            var result = await _controller.UpdateOrganisationStandardVersion(request) as OkObjectResult;
            var model = result.Value as EpaoStandardVersionResponse;

            Assert.That(result, Is.Not.Null);
            Assert.That(model.Details, Is.EqualTo("1.2"));
        }

        [Test]
        public async Task UpdateOrganisationStandardVersion_ReturnsBadRequest_OnError()
        {
            var request = new UpdateOrganisationStandardVersionRequest();
            var exMessage = "Test Exception Message";
            _mediatorMock.Setup(s => s.Send(request, It.IsAny<CancellationToken>()))
                .Throws(new Exception(exMessage));

            var result = await _controller.UpdateOrganisationStandardVersion(request) as BadRequestObjectResult;
            var response = result.Value as EpaoStandardVersionResponse;

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Details, Is.EqualTo(exMessage));
        }
    }
}
