using System.Data;
using BoDi;
using Dapper;
using DapperExtensions.Mapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    [Binding]
    public class ApiStepDefinitionBase
    {
        private readonly RestClientResult _restClientResult;
        private readonly IDbConnection _dbConnection;

        public ApiStepDefinitionBase(RestClientResult restClientResult,
                IDbConnection dbConnection
            )
        {
            _restClientResult = restClientResult;
            _dbConnection = dbConnection;
        }

        [BeforeTestRun]
        public static void SetUpBeforeTestRun()
        {
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
        }

        [BeforeFeature]
        public static void SetupBeforeFeature()
        {
            //var database = GetDatabaseInstance();
            //database.Restore();
        }

        [BeforeScenario()]
        public void SetupBeforeScenario()
        {
            var command = "DELETE FROM CertificateLogs";
            _dbConnection.Execute(command);

            command = "DELETE FROM Certificates";
            _dbConnection.Execute(command);

            command = "DELETE FROM Contacts";
            _dbConnection.Execute(command);

            command = "DELETE FROM Organisations";
            _dbConnection.Execute(command);

        }

        [Given(@"System Has access to the SFA\.DAS\.AssessmentOrgs\.Api")]
        public void GivenSystemHasAccessToTheSFA_DAS_AssessmentOrgs_Api()
        {
        }

        [Then(@"the response message should contain (.*)")]
        public void ThenTheResponseMessageShouldContain(string p0)
        {
            _restClientResult.JsonResult.Should().NotBeNull();
            _restClientResult.JsonResult.Should().Contain(p0);
        }

        [Then(@"the response http status should be (.*)")]
        public void ThenTheResponseHttpStatusShouldBe(string httpStatusCode)
        {
            _restClientResult.HttpResponseMessage.ReasonPhrase.Should().Be(httpStatusCode);
        }

        [Then(@"the Location Header should be set")]
        public void ThenTheLocationHeaderShouldBeSet()
        {
            _restClientResult.HttpResponseMessage.Headers.Location.Should().NotBeNull();
        }

        private static DatabaseUtilities GetDatabaseInstance()
        {
            var container = Bootstrapper.Container;
            var database = container.GetInstance<DatabaseUtilities>();

            return database;
        }
    }
}