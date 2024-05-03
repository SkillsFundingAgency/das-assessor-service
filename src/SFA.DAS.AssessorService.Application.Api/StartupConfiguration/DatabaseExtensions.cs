using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
            config.For<IDbConnection>().Use($"Build IDbConnection", c => {
                var tokenCredential = new DefaultAzureCredential();
                return environment.Equals(Environments.Development, StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(sqlConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = sqlConnectionString,
                        AccessToken = tokenCredential.GetTokenAsync(
                            new TokenRequestContext(scopes: new string[] { AzureResource + "/.default" }) { }
                            ).Result.ToString()
                    };
            });
            
            var option = new DbContextOptionsBuilder<AssessorDbContext>();
            config.For<AssessorDbContext>().Use(c => new AssessorDbContext(c.GetInstance<IDbConnection>(), option.Options));
        }
    }
}
