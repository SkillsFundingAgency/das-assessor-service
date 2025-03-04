using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class OrganisationQueryBase:TestBase
    {
        protected OrganisationQueryController OrganisationQueryController;
        protected UkPrnValidator UkPrnValidator;

        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        protected Mock<IMediator> Mediator = new Mock<IMediator>();

        protected Mock<IStringLocalizer<OrganisationQueryController>> OrganisationControllerLocaliserMock;

        protected Mock<ILogger<OrganisationQueryController>> ControllerLoggerMock;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;

        protected Mock<IMapper> MapperMock ;

        protected  void Setup()
        {
            Mediator = new Mock<IMediator>();

            SetupControllerMocks();

            OrganisationQueryController = new OrganisationQueryController(
                ControllerLoggerMock.Object, Mediator.Object, OrganisationQueryRepositoryMock.Object, UkPrnValidator, OrganisationControllerLocaliserMock.Object, MapperMock.Object);
        }

        private void SetupControllerMocks()
        {
            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            OrganisationControllerLocaliserMock = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<OrganisationQueryController>();

            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();

            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var ukPrnStringLocalizer = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.InvalidUkprn)
                .WithKeyValue("100000000")
                .Build<UkPrnValidator>();

            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);

            ControllerLoggerMock = new Mock<ILogger<OrganisationQueryController>>();

            MapperMock = new Mock<IMapper>();
        }
        
    }
}
