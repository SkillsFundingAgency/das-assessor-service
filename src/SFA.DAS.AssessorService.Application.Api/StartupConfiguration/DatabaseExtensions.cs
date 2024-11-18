using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class DatabaseExtensions
    {
        private const string AzureResource = "https://database.windows.net/";

        public static void AddDatabaseRegistration(this IServiceCollection services, string environment, string sqlConnectionString)
        {
            services.AddScoped<IDbConnection>(sp =>
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

            services.AddScoped<AssessorDbContext>(sp =>
            {
                var dbConnection = sp.GetRequiredService<IDbConnection>();
                var optionsBuilder = new DbContextOptionsBuilder<AssessorDbContext>();
                return new AssessorDbContext(dbConnection, optionsBuilder.Options);
            });
        }
    }
}
