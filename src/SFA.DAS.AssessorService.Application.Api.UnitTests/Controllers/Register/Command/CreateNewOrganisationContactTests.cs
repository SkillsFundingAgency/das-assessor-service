using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Command
{

    [TestFixture]
    public class CreateNewOrganisationContactTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private CreateOrganisationContactRequest _request;
        private string _orgId;
        private string _username;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _username = "username-4000";
            _orgId = "EPA999";
            _request = new CreateOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = _orgId,
                DisplayName = "Tester McTestFace",
                Email = "tester.mctestface@test.com",
                PhoneNumber = "555 5555"
            };

            _mediator.Setup(m => m.Send(_request, new CancellationToken())).ReturnsAsync(_username);

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisationContact(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationContactReturnsExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedOrganisationContactRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateOrganisationContactRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationContactShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeEpaOrganisationContactResponse()
        {
            ((OkObjectResult) _result).Value.Should().BeOfType<EpaOrganisationContactResponse>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationContactUserName()
        {
            var organisationStandardId = ((OkObjectResult) _result).Value as EpaOrganisationContactResponse;
            organisationStandardId.Details.Should().Be(_username);
        }
    }
}