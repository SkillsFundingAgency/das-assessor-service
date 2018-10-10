using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Command
{
    [TestFixture]
    public class CreateNewOrganisationContactInvalidDataTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private CreateEpaOrganisationContactRequest _request;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _request = new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = "EPA0009",
                DisplayName = "Tester McTestFace",
                Email = "tester.mctestface@test.com",
                PhoneNumber = "555 5555"
            };

            _mediator.Setup(m =>
                m.Send(_request, new CancellationToken())).Throws<BadRequestException>();

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisationContact(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationContactReturnsAnActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsInvalidOrganisatioContactRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateEpaOrganisationContactRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationContactBadRequestExceptionShouldReturnBadRequest()
        {
            _result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void BadRequestResultsAreOfTypeEpaOrganisationContactResponse()
        {
            ((BadRequestObjectResult)_result).Value.Should().BeOfType<EpaOrganisationContactResponse>();
        }
    }
}
