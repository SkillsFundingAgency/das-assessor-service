using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Data;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.AssessorService.Application.Api.IntegrationTests
{
    public static class DatabaseHelper
    {
        private static readonly string _sqlConnectionString;
        private static readonly string _sqlTestConnectionString;

        static DatabaseHelper()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();

            _sqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
            _sqlTestConnectionString = configuration.GetConnectionString("SqlConnectionStringTest");
        }

        public static AssessorDbContext TestContext
        {
            get
            {
                var sqlConnection = new SqlConnection(_sqlTestConnectionString);
                var option = new DbContextOptionsBuilder<AssessorDbContext>()
                    .EnableSensitiveDataLogging();
                return new AssessorDbContext(sqlConnection, option.Options);
            }
        }


        public static void SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"DBCC CLONEDATABASE ('SFA.DAS.AssessorService.Database', 'SFA.DAS.AssessorService.Database.ApiTest'); ALTER DATABASE [SFA.DAS.AssessorService.Database.ApiTest] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public static void DropDatabase()
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = 'SFA.DAS.AssessorService.Database.ApiTest') BEGIN ALTER DATABASE [SFA.DAS.AssessorService.Database.ApiTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE [SFA.DAS.AssessorService.Database.ApiTest]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }


        public static void Execute(string sql)
        {
            using (var connection = new SqlConnection(_sqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }


        public static T ExecuteScaler<T>(string sql)
        {
            using (var connection = new SqlConnection(_sqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.ExecuteScalar<T>(sql);
                connection.Close();

                return result;
            }
        }





        /*


                public T Get<T>(string sql)
                {
                    using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        var result = connection.Query<T>(sql);
                        connection.Close();
                        return result.FirstOrDefault();
                    }
                }

                public IEnumerable<T> GetList<T>(string sql)
                {
                    using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        var result = connection.Query<T>(sql);
                        connection.Close();
                        return result;
                    }
                }

                public object ExecuteScalar(string sql)
                {
                    using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        var result = connection.ExecuteScalar(sql);
                        connection.Close();

                        return result;
                    }
                }

                public void Execute(string sql, TestModel model)
                {
                    using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        connection.Execute(sql, model);
                        connection.Close();
                    }
                }
        */
    }
}
