using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query
{
    public class ContactsQueryBase
    {
        protected ContactQueryController ContactQueryController;
        protected UkPrnValidator UkPrnValidator;

        

        protected Mock<IContactQueryRepository> ContactQueryRepositoryMock;
        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        protected Mock<IStringLocalizer<ContactQueryController>> ContactControllerLocaliserMock;
        

        protected Mock<IStringLocalizer<SearchOrganisationForContactsValidator>>
            SearchOrganisationForContactsValidatorLocaliserMock;



        protected Mock<ILogger<ContactQueryController>> ControllerLoggerMock;

        protected Mock<IMediator> MediatorMock;

        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;
        

        private SearchOrganisationForContactsValidator _searchOrganisationForContactsValidator;

        protected void Setup()
        {
            SetupOrchestratorMocks();
            SetupControllerMocks();
            MediatorMock = new Mock<IMediator>();
            

            ContactQueryController = new ContactQueryController(
                ContactQueryRepositoryMock.Object,
                _searchOrganisationForContactsValidator,
                MediatorMock.Object,
                ControllerLoggerMock.Object, new Mock<IWebConfiguration>().Object);
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

            //GetContactsOrchestratorLocaliserMock = _mockStringLocaliserBuilder
            //    .WithKey(ResourceMessageName.NoAssesmentProviderFound)
            //    .WithKeyValue("100000000")
            //    .Build<GetContactsOrchestrator>();

            //OrchestratorLoggerMock = new Mock<ILogger<GetContactsOrchestrator>>();

            //_getContactsOrchestrator = new GetContactsOrchestrator(
            //    ContactQueryRepositoryMock.Object,
            //    OrchestratorLoggerMock.Object);

            OrganisationQueryRepositoryMock = new Mock<IOrganisationQueryRepository>();
            SearchOrganisationForContactsValidatorLocaliserMock = new Mock<IStringLocalizer<SearchOrganisationForContactsValidator>>();

            _searchOrganisationForContactsValidator = new SearchOrganisationForContactsValidator(
                OrganisationQueryRepositoryMock.Object,
                SearchOrganisationForContactsValidatorLocaliserMock.Object
            );

        }
    }
}
