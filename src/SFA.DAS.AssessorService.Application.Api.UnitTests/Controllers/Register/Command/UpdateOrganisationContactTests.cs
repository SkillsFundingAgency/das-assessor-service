using System;
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

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Command
{

        [TestFixture]
        public class UpdateOrganisationContactTests
        {
            private static RegisterController _controller;
            private static Mock<IMediator> _mediator;
            private static Mock<ILogger<RegisterController>> _logger;
            private UpdateEpaOrganisationContactRequest _request;
            private Guid _contactId;
            private object _result;

            [SetUp]
            public void Arrange()
            {
                _mediator = new Mock<IMediator>();
                _logger = new Mock<ILogger<RegisterController>>();
                _contactId = Guid.NewGuid();

                _request = new UpdateEpaOrganisationContactRequest
                {
                    ContactId = _contactId.ToString(),
                    FirstName = "name",
                    LastName = "lastname",
                    Email = "testy@test.com",
                    PhoneNumber = "12344"
                };

                _mediator.Setup(m => m.Send(_request, new CancellationToken())).ReturnsAsync(_contactId.ToString());

                _controller = new RegisterController(_mediator.Object, _logger.Object);
                _result = _controller.UpdateOrganisationContact(_request).Result;
            }

            [Test]
            public void UpdateEpaOrganisationContactReturnsExpectedActionResult()
            {
                _result.Should().BeAssignableTo<IActionResult>();
            }

            [Test]
            public void MediatorSendsExpectedOrganisationContactRequest()
            {
                _mediator.Verify(m => m.Send(It.IsAny<UpdateEpaOrganisationContactRequest>(), new CancellationToken()));
            }

            [Test]
            public void UpdateOrganisationContactShouldReturnOk()
            {
                _result.Should().BeOfType<OkObjectResult>();
            }

            [Test]
            public void ResultsAreOfTypeOrganisationContactId()
            {
                ((OkObjectResult)_result).Value.Should().BeOfType<EpaOrganisationContactResponse>();
            }

            [Test]
            public void ResultsMatchExpectedOrganisationContactId()
            {
                var organisationStandardId = ((OkObjectResult)_result).Value as EpaOrganisationContactResponse;
                organisationStandardId.Details.Should().Be(_contactId.ToString());
            }       
    }
}
