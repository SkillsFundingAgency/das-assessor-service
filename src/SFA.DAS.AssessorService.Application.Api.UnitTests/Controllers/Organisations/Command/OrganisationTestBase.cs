using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Command
{
    public class OrganisationTestBase
    {               
        protected Mock<ILogger<OrganisationController>> ControllerLoggerMock;  
        protected Mock<IMediator> Mediator;
        
        protected OrganisationController OrganisationController;
      
        protected void Setup()
        {
            SetupOrchestratorMocks();
            SetupControllerMocks();
            
            OrganisationController = new OrganisationController(ControllerLoggerMock.Object, Mediator.Object, Mock.Of<IWebConfiguration>());           
        }

        private void SetupOrchestratorMocks()
        {   
            Mediator = new Mock<IMediator>();
        }

        private void SetupControllerMocks()
        {          
            ControllerLoggerMock = new Mock<ILogger<OrganisationController>>();
        }
    }
}
