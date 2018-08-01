using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Services
{
    public class DatabaseService
    {
       // A typical connectionString stored in our config: "Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.AssessorService.Database;Integrated Security=True; MultipleActiveResultSets=True;";
        private readonly string _sqlConnectionStringWithoutMars =
            $@"Data Source = (localdb)\ProjectsV13; Initial Catalog = SFA.DAS.AssessorService.Database; Integrated Security = True; MultipleActiveResultSets = False;";

        public string SqlTestConnectionString { get; } =
            $@"Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.AssessorService.Database.Test;Integrated Security=True; MultipleActiveResultSets=True;";

        public AssessorDbContext TestContext
        {
            get
            {
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(new DatabaseService().SqlTestConnectionString, options => options.EnableRetryOnFailure(3));
                return new AssessorDbContext(option.Options);
            }
        }
        public void SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(_sqlConnectionStringWithoutMars))
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
            using (var connection = new SqlConnection(SqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();             
                connection.Execute(sql);
                connection.Close();
            }
        }

        public void Execute(string sql, TestModel model)
        {
            using (var connection = new SqlConnection(SqlTestConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(_sqlConnectionStringWithoutMars))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = 'SFA.DAS.AssessorService.Database.Test') DROP DATABASE [SFA.DAS.AssessorService.Database.Test];"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }
    }
}
