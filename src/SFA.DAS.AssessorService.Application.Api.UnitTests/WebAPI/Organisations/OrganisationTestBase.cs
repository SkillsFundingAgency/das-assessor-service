using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations
{
    public class OrganisationTestBase
    {               
        protected Mock<IStringLocalizer<OrganisationOrchestrator>> OrganisationsOrchestratorLocaliserMock;
        protected Mock<ILogger<OrganisationController>> ControllerLoggerMock;  
        protected Mock<IMediator> Mediator;
        
        protected OrganisationController OrganisationController;
        private OrganisationOrchestrator _organisationOrchestrator;
      
        protected void Setup()
        {
            SetupOrchestratorMocks();

            SetupControllerMocks();

            OrganisationController = new OrganisationController(
                _organisationOrchestrator, ControllerLoggerMock.Object);           
        }

        private void SetupOrchestratorMocks()
        {         
            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            OrganisationsOrchestratorLocaliserMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<OrganisationOrchestrator>();

            Mediator = new Mock<IMediator>();

            _organisationOrchestrator = new OrganisationOrchestrator(
                Mediator.Object,
                OrganisationsOrchestratorLocaliserMock.Object);
        }

        private void SetupControllerMocks()
        {          
            ControllerLoggerMock = new Mock<ILogger<OrganisationController>>();
        }
    }
}
