using System.Net.Http;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Startup.DependencyResolution
{
    public class NotificationsRegistry : Registry
    {
        public NotificationsRegistry()
        {
            INotificationsApiClientConfiguration clientConfiguration = new NotificationsApiClientConfiguration
            {
                ApiBaseUrl = "https://at-notifications.apprenticeships.sfa.bis.gov.uk/",
                ClientToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJkYXRhIjoiU2VuZFNtcyBTZW5kRW1haWwiLCJpc3MiOiJodHRwOi8vZGFzLWF0LW5vdC1jcyIsImF1ZCI6Imh0dHA6Ly9kYXMtYXQtZWFzLWNzIiwiZXhwIjoxNTU1NzU2OTQ4LCJuYmYiOjE1MjMzNTY5NDh9.2IKr0p-nq5KscucjgzhXrfbVQ_mdQ63yH3PLSVSZ9Xk",
                ClientId = "",
                ClientSecret = "",
                IdentifierUri = "",
                Tenant = ""
            };

            //var config = ConfigurationHelper.GetConfiguration<Domain.Configuration.NotificationsApiClientConfiguration>($"{Constants.ServiceName}.Notifications");

            var httpClient = string.IsNullOrWhiteSpace(clientConfiguration.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(clientConfiguration)).Build()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(clientConfiguration)).Build();

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);
            For<INotificationsApiClientConfiguration>().Use(clientConfiguration);
        }
    }
}