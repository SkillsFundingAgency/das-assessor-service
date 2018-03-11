using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Contacts.Queries.Get
{
    public class ContactsQueryBase
    {
        protected ContactQueryController ContactQueryController;
        protected UkPrnValidator UkPrnValidator;

        protected Mock<GetContactsOrchestrator> GetContactsOrchestratorMock;

        protected Mock<IContactQueryRepository> ContactQueryRepositoryMock;

        protected Mock<IStringLocalizer<ContactQueryController>> ContactControllerLocaliserMock;
        protected Mock<IStringLocalizer<GetContactsOrchestrator>> GetContactsOrchestratorLocaliserMock;

        protected Mock<ILogger<ContactQueryController>> ControllerLoggerMock;
        protected Mock<ILogger<GetContactsOrchestrator>> OrchestratorLoggerMock;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;
        private GetContactsOrchestrator _getContactsOrchestrator;

        private SearchOrganisationForContactsValidator _searchOrganisationForContactsValidator;

        protected void Setup()
        {
            SetupOrchestratorMocks();
            SetupControllerMocks();

            ContactQueryController = new ContactQueryController(
                _getContactsOrchestrator,
                _searchOrganisationForContactsValidator,
                ControllerLoggerMock.Object);
        }

        private void SetupControllerMocks()
        {
            ContactControllerLocaliserMock = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<ContactQueryController>();

            ControllerLoggerMock = new Mock<ILogger<ContactQueryController>>();
        }

        private void SetupOrchestratorMocks()
        {
            ContactQueryRepositoryMock = new Mock<IContactQueryRepository>();

            _mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            var ukPrnStringLocalizer = _mockStringLocaliserBuilder
                    .WithKey(ResourceMessageName.InvalidUkprn)
                .WithKeyValue("100000000")
                .Build<UkPrnValidator>();

            UkPrnValidator = new UkPrnValidator(ukPrnStringLocalizer.Object);

            GetContactsOrchestratorLocaliserMock = _mockStringLocaliserBuilder
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<GetContactsOrchestrator>();

            OrchestratorLoggerMock = new Mock<ILogger<GetContactsOrchestrator>>();

            _getContactsOrchestrator = new GetContactsOrchestrator(
                ContactQueryRepositoryMock.Object,
                OrchestratorLoggerMock.Object);
        }
    }
}
