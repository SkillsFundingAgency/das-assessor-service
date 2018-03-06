using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BoDi;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    [Binding]
    public class SpecflowIocRegister
    {
        [BeforeTestRun]
        public static void RegisterTypes(IObjectContainer objectContainer)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString]
                .ConnectionString;

            var sqlConnection = new SqlConnection(connectionString);
            objectContainer.RegisterInstanceAs<IDbConnection>(sqlConnection);
        }
    }
}