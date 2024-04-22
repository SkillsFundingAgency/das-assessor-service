using Microsoft.Azure.Services.AppAuthentication;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Settings;


namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class NServiceBusRegistration
    {
        public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
        {
            //if (configuration.IsIntegrationTests()) return serviceProvider;

            var appSettings = configuration.GetSection("ApplicationSettings").Get<ApiConfiguration>();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.AssessorService.Application.Api")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => CreateSqlConnection(configuration, appSettings))
                .UseSendOnly();

            if (appSettings.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
            {
                endpointConfiguration.UseTransport<LearningTransport>();
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(appSettings.NServiceBusConnectionString, r => r.AddRouting());
            }

            if (!string.IsNullOrEmpty(appSettings.NServiceBusLicense))
            {
                endpointConfiguration.License(appSettings.NServiceBusLicense);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();

            return serviceProvider;
        }

        private static DbConnection CreateSqlConnection(IConfiguration configuration, ApiConfiguration appSettings)
        {
            if (appSettings != null)
            {
                return new SqlConnection(appSettings.SqlConnectionString);
            }
            else
            {
                return new SqlConnection
                {
                    ConnectionString = appSettings.SqlConnectionString,
                    AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result
                };
            }
        }
    }
}
