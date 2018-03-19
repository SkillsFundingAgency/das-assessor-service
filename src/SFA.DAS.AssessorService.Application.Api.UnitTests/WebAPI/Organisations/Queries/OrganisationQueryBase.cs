using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Api.UnitTests.Helpers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Queries
{
    public class OrganisationQueryBase
    {
        protected OrganisationQueryController OrganisationQueryController;
        protected UkPrnValidator UkPrnValidator;

       // protected Mock<GetOrganisationsOrchestrator> GetOrganisationsOrchestratorMock;

        protected Mock<IOrganisationQueryRepository> OrganisationQueryRepositoryMock;

        protected Mock<IStringLocalizer<OrganisationQueryController>> OrganisationControllerLocaliserMock;
        //protected Mock<IStringLocalizer<GetOrganisationsOrchestrator>> GetOrganisationsOrchestratorLocaliserMock;
           
        protected Mock<ILogger<OrganisationQueryController>> ControllerLoggerMock;
       // protected Mock<ILogger<GetOrganisationsOrchestrator>> OrchestratorLoggerMock;
      
        private MockStringLocaliserBuilder _mockStringLocaliserBuilder;
        ////private GetOrganisationsOrchestrator _getOrganisationsOrchestrator;

        protected  void Setup()
        {
            //SetupOrchestratorMocks();
            SetupControllerMocks();

            OrganisationQueryController = new OrganisationQueryController(ControllerLoggerMock.Object, OrganisationQueryRepositoryMock.Object, UkPrnValidator, OrganisationControllerLocaliserMock.Object);
        }

        private void SetupControllerMocks()
        {
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
        }

        //private void SetupOrchestratorMocks()
        //{         
           

            

        //    GetOrganisationsOrchestratorLocaliserMock = _mockStringLocaliserBuilder
        //        .WithKey(ResourceMessageName.NoAssesmentProviderFound)
        //        .WithKeyValue("100000000")
        //        .Build<GetOrganisationsOrchestrator>();

        //    OrchestratorLoggerMock = new Mock<ILogger<GetOrganisationsOrchestrator>>();

        //    _getOrganisationsOrchestrator = new GetOrganisationsOrchestrator(
        //        OrganisationQueryRepositoryMock.Object,
        //        GetOrganisationsOrchestratorLocaliserMock.Object,
        //        UkPrnValidator,
        //        OrchestratorLoggerMock.Object);
        //}
    }
}
