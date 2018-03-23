using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.RegisterImport
{
    [TestFixture]
    public class WhenPostToImport
    {
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterImportController>>();

            _controller = new RegisterImportController(_mediator.Object, _logger.Object);
        }

        [Test]
        public void Returns_A_Result()
        {
            _result = _controller.Import().Result;
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void Sends_A_Message_To_Start_Import() 
        {
            _result = _controller.Import().Result;
            _mediator.Verify(m => m.Send(It.IsAny<RegisterUpdateRequest>(), new CancellationToken()));
        }

        private static RegisterImportController _controller;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterImportController>> _logger;
    }
}