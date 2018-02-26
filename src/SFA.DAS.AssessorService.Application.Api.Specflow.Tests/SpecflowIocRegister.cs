namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using BoDi;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using TechTalk.SpecFlow;

    [Binding]
    public class SpecflowIocRegister
    {
        [BeforeTestRun]
        public static void RegisterTypes(IObjectContainer objectContainer)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString].ConnectionString;

            var sqlConnection = new SqlConnection(connectionString);
            objectContainer.RegisterInstanceAs<IDbConnection>(sqlConnection);
        }
    }
}
