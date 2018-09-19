using System;
using System.Threading;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Command
{
    [TestFixture]
    public class CreateOrganisationStandardTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private CreateEpaOrganisationStandardRequest _request;
        private string _orgId;
        private string _organisationStandardId;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _organisationStandardId = "4000";
            _orgId = "EPA999";
            _request = new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = _orgId,
                StandardCode = 3,
               EffectiveFrom = DateTime.Now,
               Comments = "this is a comment"
            };

            _mediator.Setup(m => m.Send(_request, new CancellationToken())).ReturnsAsync(_organisationStandardId);

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisationStandard(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationStandardReturnsExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedOrganisationStandardRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateEpaOrganisationStandardRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationStandardShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeEpaOrganisationStandardResponse()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<EpaOrganisationStandardResponse>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationStandardId()
        {
            var organisationStandardId = ((OkObjectResult)_result).Value as EpaOrganisationStandardResponse;
            organisationStandardId.Details.Should().Be(_organisationStandardId);
        }
    }
}
