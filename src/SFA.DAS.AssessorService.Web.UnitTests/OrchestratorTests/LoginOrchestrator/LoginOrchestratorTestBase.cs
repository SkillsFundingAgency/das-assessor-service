//using Microsoft.Extensions.Logging;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.AssessorService.Application.Api.Client.Clients;
//using SFA.DAS.AssessorService.Settings;

//namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
//{
//    public class LoginOrchestratorTestBase
//    {
//        protected Orchestrators.Login.LoginOrchestrator LoginOrchestrator;
//        protected Mock<IOrganisationsApiClient> OrganisationsApiClient;

//        [SetUp]
//        protected void Setup()
//        {
//            var config = new WebConfiguration() { Authentication = new AuthSettings() { Role = "EPA" } };

//            OrganisationsApiClient = new Mock<IOrganisationsApiClient>();
//            LoginOrchestrator = new Orchestrators.Login.LoginOrchestrator(config,
//                OrganisationsApiClient.Object, new Mock<IContactsApiClient>().Object, new Mock<ILogger<Orchestrators.Login.LoginOrchestrator>>().Object);
//        }
//    }
//}