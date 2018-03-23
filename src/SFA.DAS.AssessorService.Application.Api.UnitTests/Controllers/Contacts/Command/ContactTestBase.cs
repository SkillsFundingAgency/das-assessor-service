using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Controllers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Command
{
    public class ContactTestBase
    {               
        protected Mock<ILogger<ContactController>> ControllerLoggerMock;  
        protected Mock<IMediator> Mediator;        
        protected ContactController ContactController;
      
        protected void Setup()
        {
            SetupOrchestratorMocks();

            SetupControllerMocks();

            ContactController = new ContactController(ControllerLoggerMock.Object, Mediator.Object);           
        }

        private void SetupOrchestratorMocks()
        {   
            Mediator = new Mock<IMediator>();
        }

        private void SetupControllerMocks()
        {          
            ControllerLoggerMock = new Mock<ILogger<ContactController>>();
        }
    }
}
