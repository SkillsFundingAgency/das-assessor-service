using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data;
using StructureMap;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class DatabaseExtensions
    {
        private const string AzureResource = "https://database.windows.net/";

        public static void AddDatabaseRegistration(this ConfigurationExpression config, string environment, string sqlConnectionString)
        {
            config.For<IDbConnection>().Use($"Build IDbConnection", c =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                return environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(sqlConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = sqlConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            var option = new DbContextOptionsBuilder<AssessorDbContext>();
            config.For<AssessorDbContext>().Use(c => new AssessorDbContext(c.GetInstance<IDbConnection>(), option.Options));
        }
    }
}
