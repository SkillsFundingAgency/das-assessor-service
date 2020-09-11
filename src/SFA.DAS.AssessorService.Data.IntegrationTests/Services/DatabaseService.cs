using Dapper;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Settings;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{
    public class DatabaseService
    {
     
        public DatabaseService()
        {
           
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();
            WebConfiguration = new TestWebConfiguration
            {
                SqlConnectionString = Configuration.GetConnectionString("SqlConnectionStringTest")
            };
        }

        public AssessorDbContext TestContext
        {
            get
            {
                var sqlConnectionStringTest = Configuration.GetConnectionString("SqlConnectionStringTest");
                var sqlConnectionTest = new SqlConnection(sqlConnectionStringTest)
                {
                    AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result
                };
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(sqlConnectionTest, options => options.EnableRetryOnFailure(3));
                return new AssessorDbContext(option.Options);
            }
        }


        public IConfiguration Configuration { get; }
        public TestWebConfiguration WebConfiguration;

        public void SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"DBCC CLONEDATABASE ('SFA.DAS.AssessorService.Database', 'SFA.DAS.AssessorService.Database.Test'); ALTER DATABASE [SFA.DAS.AssessorService.Database.Test] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public void Execute(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.Query<T>(sql);
                connection.Close();
                return result.FirstOrDefault();
            }    
        }

        
        public object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

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
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = 'SFA.DAS.AssessorService.Database.Test') BEGIN ALTER DATABASE [SFA.DAS.AssessorService.Database.Test] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE [SFA.DAS.AssessorService.Database.Test]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }
    }
}