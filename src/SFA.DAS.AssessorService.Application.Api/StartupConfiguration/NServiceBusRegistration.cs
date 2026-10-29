using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    [ExcludeFromCodeCoverage]
    public static class NServiceBusRegistration
    {
        private const string EndpointName = "SFA.DAS.AssessorService.Api";
        private const string LearningEndpointConnectionString = "UseLearningEndpoint=true";
        public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, IApiConfiguration configuration)
        {
            if (configuration.IsIntegrationTests()) return serviceProvider;


            var endpointConfiguration = new EndpointConfiguration(EndpointName)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => CreateSqlConnection(configuration))
                .UseSendOnly();

            if (configuration.NServiceBusConnectionString.Equals(LearningEndpointConnectionString, StringComparison.CurrentCultureIgnoreCase))
            {
                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                transport.StorageDirectory(LearningStorageDirectory());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(configuration.NServiceBusConnectionString);
            }

            if (!string.IsNullOrEmpty(configuration.NServiceBusLicense))
            {
                endpointConfiguration.License(configuration.NServiceBusLicense);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();

            return serviceProvider;
        }

        private static string LearningStorageDirectory() =>
            Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
    @"src\.learningtransport");

        private static DbConnection CreateSqlConnection(IApiConfiguration configuration)
        {
            if (configuration.IsLocalAcceptanceOrDev())
            {
                return new SqlConnection(configuration.SqlConnectionString);
            }
            else
            {
                return new SqlConnection
                {
                    ConnectionString = configuration.SqlConnectionString,
                    AccessToken = new DefaultAzureCredential().GetTokenAsync(
                        new TokenRequestContext(scopes: new string[] { "https://database.windows.com/" })).Result.Token
                };
            }
        }
    }
}
