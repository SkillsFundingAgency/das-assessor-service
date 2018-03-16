using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils
{
    public class DatabaseUtilities
    {
        readonly SqlConnection _connection;
        private readonly string _databaseBackupLocation;
        private readonly string _databaseName;

        public DatabaseUtilities()
        {
            var settings = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString];
            _connection = new SqlConnection(settings.ConnectionString);

            _databaseBackupLocation = ConfigurationManager.AppSettings[PersistenceNames.RestoreDatabase];
            _databaseName = ConfigurationManager.AppSettings[PersistenceNames.DatabaseName];
        }

        public void Backup()
        {
            var path = Path.GetDirectoryName(_databaseBackupLocation);
            if (!System.IO.Directory.Exists(path))
                Directory.CreateDirectory(path);

            _connection.Open();
            var sqlcmd = new SqlCommand("backup database " + _databaseName + " to disk='" + _databaseBackupLocation + "'", _connection);
            sqlcmd.ExecuteNonQuery();
            _connection.Close();
        }

        public void Restore()
        {
            var database = _connection.Database;
            if (_connection.State != ConnectionState.Open) _connection.Open();

            var setSingleUserModeSqlStatement =
                string.Format("ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            var bu2 = new SqlCommand(setSingleUserModeSqlStatement, _connection);
            bu2.ExecuteNonQuery();

            var restoreDatabaseSqlStatement = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" +
                                              _databaseBackupLocation + "'WITH REPLACE;";
            var bu3 = new SqlCommand(restoreDatabaseSqlStatement, _connection);
            bu3.ExecuteNonQuery();

            var setMultiUserModeSqlStatement = string.Format("ALTER DATABASE [" + database + "] SET MULTI_USER");
            var bu4 = new SqlCommand(setMultiUserModeSqlStatement, _connection);
            bu4.ExecuteNonQuery();

            _connection.Close();
        }
    }
}