using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

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
                return new AssessorDbContext(new SqlConnection(sqlConnectionStringTest), option.Options);
            }
        }


        public IConfiguration Configuration { get; }
        public TestWebConfiguration WebConfiguration;

        public async Task SetupDatabase()
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
                        @"DBCC CLONEDATABASE ('SFA.DAS.AssessorService.Database', 'SFA.DAS.AssessorService.Database.Test'); " + 
                            " ALTER DATABASE [SFA.DAS.AssessorService.Database.Test] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }

            await LookupDataHelper.AddLookupData();
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

        public async Task<T> QueryFirstOrDefaultAsync<T, M>(string sql, M model) where T : TestModel
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param: model);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql);
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

        public void Execute<T>(string sql, T model)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                connection.Execute(sql, model);
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