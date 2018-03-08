using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils
{
    public class DatabaseUtilities
    {
        public void Restore()
        {
            var settings = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString];
            var connection = new SqlConnection(settings.ConnectionString);

            var restoreDatabase = ConfigurationManager.AppSettings[PersistenceNames.RestoreDatabase];

            var database = connection.Database;
            if (connection.State != ConnectionState.Open) connection.Open();

            var setSingleUserModeSqlStatement =
                string.Format("ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            var bu2 = new SqlCommand(setSingleUserModeSqlStatement, connection);
            bu2.ExecuteNonQuery();

            var restoreDatabaseSqlStatement = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" +
                                              restoreDatabase + "'WITH REPLACE;";
            var bu3 = new SqlCommand(restoreDatabaseSqlStatement, connection);
            bu3.ExecuteNonQuery();

            var setMultiUserModeSqlStatement = string.Format("ALTER DATABASE [" + database + "] SET MULTI_USER");
            var bu4 = new SqlCommand(setMultiUserModeSqlStatement, connection);
            bu4.ExecuteNonQuery();

            connection.Close();
        }
    }
}