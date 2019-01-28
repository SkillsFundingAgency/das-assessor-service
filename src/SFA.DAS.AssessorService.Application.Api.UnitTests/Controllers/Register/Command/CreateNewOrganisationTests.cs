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
    public class CreateNewOrganisationTests
    {
        private static RegisterController _controller;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterController>> _logger;
        private object _result;
        private CreateEpaOrganisationRequest _request;
        private string _organisationId;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterController>>();

            _organisationId = "EPA999";
            _request = new CreateEpaOrganisationRequest
            {
                Name = "name 1",
                Ukprn = 123321,
                OrganisationTypeId = 5,
                LegalName = "legal name 1",
                WebsiteLink = "website link 1",
                Address1 = "address 1",
                Address2 = "address 2",
                Address3 = "address 3",
                Address4 = "address 4",
                Postcode = "postcode",
                CompanyNumber = "company number",
                CharityNumber = "charity number"
            };

            _mediator.Setup(m =>
                m.Send(_request, new CancellationToken())).ReturnsAsync(_organisationId);

            _controller = new RegisterController(_mediator.Object, _logger.Object);
            _result = _controller.CreateOrganisation(_request).Result;
        }

        [Test]
        public void CreateEpaOrganisationReturnsExpectedActionResult()
        {
            _result.Should().BeAssignableTo<IActionResult>();
        }

        [Test]
        public void MediatorSendsExpectedOrganisationRequest()
        {
            _mediator.Verify(m => m.Send(It.IsAny<CreateEpaOrganisationRequest>(), new CancellationToken()));
        }

        [Test]
        public void CreateOrganisationShouldReturnOk()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void ResultsAreOfTypeEpaOrganisation()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<EpaOrganisationResponse>();
        }

        [Test]
        public void ResultsMatchExpectedOrganisation()
        {
            var organisation = ((OkObjectResult)_result).Value as EpaOrganisationResponse;
            organisation.Details.Should().BeEquivalentTo(_organisationId);
        }
    }
}