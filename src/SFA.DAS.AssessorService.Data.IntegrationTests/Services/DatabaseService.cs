using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Settings;

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
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(sqlConnectionStringTest, options => options.EnableRetryOnFailure(3));
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
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var res = connection.Query<T>(sql);
                connection.Close();

                return res.FirstOrDefault();
            }    
        }

        
        public object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var res = connection.ExecuteScalar(sql);
                connection.Close();

                return res;
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


        public void DropDatabase()
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
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