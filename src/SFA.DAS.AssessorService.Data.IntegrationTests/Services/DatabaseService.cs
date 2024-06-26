﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();

            SqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
            SqlConnectionStringTest = configuration.GetConnectionString("SqlConnectionStringTest");
        }

        public AssessorDbContext TestContext
        {
            get
            {
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(SqlConnectionStringTest, options => options.EnableRetryOnFailure(3));
                return new AssessorDbContext(new SqlConnection(SqlConnectionStringTest), option.Options);
            }
        }

        public string SqlConnectionString { get; }
        public string SqlConnectionStringTest { get; }

        public async Task SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        @"DBCC CLONEDATABASE ('SFA.DAS.AssessorService.Database', 'SFA.DAS.AssessorService.Database.Test'); " + 
                         "ALTER DATABASE [SFA.DAS.AssessorService.Database.Test] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }

            await LookupDataHelper.AddLookupData();
        }

        public void Execute(string sql)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                connection.Execute(sql);
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                var result = connection.Query<T>(sql);
                return result.FirstOrDefault();
            }    
        }

        public IEnumerable<T> GetList<T>(string sql)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                var result = connection.Query<T>(sql);
                return result;
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object model) //where T : TestModel
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param: model);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql);
            }
        }

        public object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                var result = connection.ExecuteScalar(sql);
                return result;
            }
        }

        public void Execute<T>(string sql, T model)
        {
            using (var connection = new SqlConnection(SqlConnectionStringTest))
            {
                connection.Execute(sql, model);
            }
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(SqlConnectionString))
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