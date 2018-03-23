using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts
{
    public class ContactTestBase
    {               
        protected Mock<IStringLocalizer<ContactOrchestrator>> ContactsOrchestratorLocaliserMock;
        protected Mock<ILogger<ContactController>> ControllerLoggerMock;  
        protected Mock<IMediator> Mediator;
        
        protected ContactController ContactController;
        private ContactOrchestrator _contactOrchestrator;
      
        protected void Setup()
        {
            SetupOrchestratorMocks();

            SetupControllerMocks();

            ContactController = new ContactController(
                _contactOrchestrator, ControllerLoggerMock.Object);           
        }

        private void SetupOrchestratorMocks()
        {         
            var mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            ContactsOrchestratorLocaliserMock = mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<ContactOrchestrator>();

            Mediator = new Mock<IMediator>();

            _contactOrchestrator = new ContactOrchestrator(
                Mediator.Object,
                ContactsOrchestratorLocaliserMock.Object);
        }

        private void SetupControllerMocks()
        {          
            ControllerLoggerMock = new Mock<ILogger<ContactController>>();
        }
    }
}
