using System;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Command
{
    [TestFixture]
    public class CreateOrganisationStandardAlreadyExistsTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private CreateEpaOrganisationStandardRequest _request;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _request = new CreateEpaOrganisationStandardRequest
            { 
                OrganisationId = "EPA999",
                StandardCode = 3,
                EffectiveFrom = DateTime.Now,
                Comments = "this is a comment"
            };

            _mediator.Setup(m =>
                m.Send(_request, new CancellationToken())).Throws<AlreadyExistsException>();

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisationStandard(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationStandardReturnsAnActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedOrganisationStandardRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateEpaOrganisationStandardRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationStandardAlreadyExistsExceptionShouldReturnConflict()
        {
            _result.Should().BeOfType<ConflictObjectResult>();
        }

        [Test]
        public void BadRerquestResultsAreOfTypeString()
        {
            ((ConflictObjectResult)_result).Value.Should().BeOfType<string>();
        }
    }
}
