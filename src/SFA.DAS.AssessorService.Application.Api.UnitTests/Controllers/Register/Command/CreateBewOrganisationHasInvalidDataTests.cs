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
    public class CreateBewOrganisationHasInvalidDataTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private EpaOrganisation _returnedOrganisation;
        private CreateEpaOrganisationRequest _request;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _request = new CreateEpaOrganisationRequest
            {
                Name = "name 1",
                OrganisationId = "EPA999",
                Ukprn = 123321,
                OrganisationTypeId = 5,
                LegalName = "legal name 1",
                WebsiteLink = "website link 1",
                Address1 = "address 1",
                Address2 = "address 2",
                Address3 = "address 3",
                Address4 = "address 4",
                Postcode = "postcode"
            };

            _returnedOrganisation = new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = _request.Name,
                OrganisationId = _request.OrganisationId,
                Ukprn = _request.Ukprn,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = _request.OrganisationTypeId,
                OrganisationData = new OrganisationData
                {
                    LegalName = _request.LegalName,
                    Address1 = _request.Address1,
                    Address2 = _request.Address2,
                    Address3 = _request.Address3,
                    Address4 = _request.Address4,
                    Postcode = _request.Postcode
                }
            };

            _mediator.Setup(m =>
                m.Send(_request, new CancellationToken())).Throws<BadRequestException>();

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisation(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationReturnsAnActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsInvalidOrganisationRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateEpaOrganisationRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationBadRequestExceptionShouldReturnBadRequest()
        {
            _result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void BadRerquestResultsAreOfTypeString()
        {
            ((BadRequestObjectResult)_result).Value.Should().BeOfType<string>();
        }
    }
}
