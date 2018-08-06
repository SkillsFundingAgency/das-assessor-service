using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{
    public class DatabaseService
    {
       //// A typical connectionString stored in our config: "Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.AssessorService.Database;Integrated Security=True; MultipleActiveResultSets=True;";
       // private readonly string _sqlConnectionStringWithoutMars =
       //     $@"Data Source = (localdb)\ProjectsV13; Initial Catalog = SFA.DAS.AssessorService.Database; Integrated Security = True; MultipleActiveResultSets = False;";

       // public string _sqlTestConnectionString { get; } =
       //     $@"Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.AssessorService.Database.Test;Integrated Security=True; MultipleActiveResultSets=True;";
        private readonly IConfiguration _configuration;

        public DatabaseService()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();
        }

        public AssessorDbContext TestContext
        {
            get
            {
                var sqlConnectionStringTest = _configuration.GetConnectionString("SqlConnectionStringTest");
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(sqlConnectionStringTest, options => options.EnableRetryOnFailure(3));
                return new AssessorDbContext(option.Options);
            }
        }
        public void SetupDatabase()
        {
            DropDatabase();

            var sqlConnectionStringWithoutMars = _configuration.GetConnectionString("SqlConnectionString");

            using (var connection = new SqlConnection(sqlConnectionStringWithoutMars))
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
            var sqlTestConnectionString = _configuration.GetConnectionString("SqlConnectionStringTest");
            using (var connection = new SqlConnection(sqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();             
                connection.Execute(sql);
                connection.Close();
            }
        }

        public void Execute(string sql, TestModel model)
        {
            var sqlTestConnectionString = _configuration.GetConnectionString("SqlConnectionStringTest");
            using (var connection = new SqlConnection(sqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            var sqlConnectionStringWithoutMars = _configuration.GetConnectionString("SqlConnectionString");
            using (var connection = new SqlConnection(sqlConnectionStringWithoutMars))
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
