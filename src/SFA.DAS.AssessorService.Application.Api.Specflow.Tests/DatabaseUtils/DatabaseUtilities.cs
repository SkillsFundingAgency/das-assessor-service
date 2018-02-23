namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils
{
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class DatabaseUtilities
    {
        public void Restore()
        {
            var settings = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString];
            var connection = new SqlConnection(settings.ConnectionString);

            var restoreDatabase = ConfigurationManager.AppSettings[PersistenceNames.RestoreDatabase];

            string database = connection.Database.ToString();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            string setSingleUserModeSqlStatement = string.Format("ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            SqlCommand bu2 = new SqlCommand(setSingleUserModeSqlStatement, connection);
            bu2.ExecuteNonQuery();

            string restoreDatabaseSqlStatement = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" + restoreDatabase + "'WITH REPLACE;";
            SqlCommand bu3 = new SqlCommand(restoreDatabaseSqlStatement, connection);
            bu3.ExecuteNonQuery();

            string setMultiUserModeSqlStatement = string.Format("ALTER DATABASE [" + database + "] SET MULTI_USER");
            SqlCommand bu4 = new SqlCommand(setMultiUserModeSqlStatement, connection);
            bu4.ExecuteNonQuery();

            connection.Close();

        }
    }
}

