using System.Threading;
using FluentAssertions;
using Machine.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using It = Moq.It;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.RegisterImport
{


    [Subject("AssessorService")]
    public class WhenPostToImport
    {
        private Establish context = () =>
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterImportController>>();

            _controller = new RegisterImportController(_mediator.Object, _logger.Object);
        };

        private Because of = () =>
        {
            _result = _controller.Import().Result;
        };

        private Machine.Specifications.It Returns_A_Result = () =>
        {
            _result.Should().BeAssignableTo<IActionResult>();
        };

        private Machine.Specifications.It Sends_A_Message_To_Start_Import = () =>
        {
            _mediator.Verify(m => m.Send(It.IsAny<RegisterUpdateRequest>(), new CancellationToken()));
        };

        private static RegisterImportController _controller;
        private static object _result;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterImportController>> _logger;
    }
}