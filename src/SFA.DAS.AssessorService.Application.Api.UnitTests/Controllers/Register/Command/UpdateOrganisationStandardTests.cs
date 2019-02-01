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
    public class UpdateOrganisationStandardTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private UpdateEpaOrganisationStandardRequest _request;
        private string _orgId;
        private int _standardCode;
        private int _organisationStandardId;
        private object _result;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _organisationStandardId = 4000;
            _standardCode = 3;
            _orgId = "EPA999";
            _request = new UpdateEpaOrganisationStandardRequest
            {
                OrganisationId = _orgId,
                StandardCode = _standardCode,
                EffectiveFrom = DateTime.Now,
                Comments = "this is a comment",
                EffectiveTo = null
            };

            _mediator.Setup(m => m.Send(_request, new CancellationToken())).ReturnsAsync(_organisationStandardId.ToString());

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.UpdateOrganisationStandard(_request).Result;
        }


        [Test]
        public void UpdateEpaOrganisationStandardReturnsExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedOrganisationStandardRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<UpdateEpaOrganisationStandardRequest>(), new CancellationToken()));
        }

        [Test]
        public void UpdateeOrganisationStandardShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeOrganisationStandardId()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<EpaoStandardResponse>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisationStandardId()
        {
            var organisationStandardId = ((OkObjectResult)_result).Value as EpaoStandardResponse;
            organisationStandardId.Details.Should().Be(_organisationStandardId.ToString());
        }
    }
}
