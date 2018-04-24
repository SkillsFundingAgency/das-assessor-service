﻿using System.Net.Http;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoImporter.Startup.DependencyResolution
{
    public class NotificationsRegistry : Registry
    {
        public NotificationsRegistry()
        {
            var configuration = ConfigurationHelper.GetConfiguration();

            INotificationsApiClientConfiguration clientConfiguration = new NotificationsApiClientConfiguration
            {
                ApiBaseUrl = configuration.NotificationsApiClientConfiguration.ApiBaseUrl,
                ClientToken = configuration.NotificationsApiClientConfiguration.ClientToken,
                ClientId = "",
                ClientSecret = "",
                IdentifierUri = "",
                Tenant = ""
            };

            var httpClient = string.IsNullOrWhiteSpace(clientConfiguration.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(clientConfiguration)).Build()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(clientConfiguration)).Build();

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);
            For<INotificationsApiClientConfiguration>().Use(clientConfiguration);
        }
    }
}