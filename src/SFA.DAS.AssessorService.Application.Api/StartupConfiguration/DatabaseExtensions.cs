﻿using System;
using System.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class DatabaseExtensions
    {
        private const string AzureResource = "https://database.windows.net/";

        public static void AddDatabaseRegistration(this IServiceCollection services, bool useSandbox, IApiConfiguration config)
        {
            var sqlConnectionString = useSandbox ? config.SandboxSqlConnectionString : config.SqlConnectionString;

            services.AddScoped<IDbConnection>(sp =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return config.Environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(sqlConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = sqlConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            services.AddScoped<IAssessorDbContext>(sp =>
            {
                var dbConnection = sp.GetRequiredService<IDbConnection>();
                var optionsBuilder = new DbContextOptionsBuilder<AssessorDbContext>();

                return new AssessorDbContext(dbConnection, optionsBuilder.Options);
            });
        }
    }
}
