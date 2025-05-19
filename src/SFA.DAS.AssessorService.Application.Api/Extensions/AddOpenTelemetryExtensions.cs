using Microsoft.Extensions.DependencyInjection;
using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace SFA.DAS.AssessorService.Application.Api.Extensions
{
    public static class AddOpenTelemetryExtensions
    {
        /// <summary>
        /// Add the OpenTelemetry telemetry service to the application.
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="appInsightsConnectionString">Azure app insights connection string.</param>
        public static void AddOpenTelemetryRegistration(this IServiceCollection services, string appInsightsConnectionString)
        {
            if (!string.IsNullOrEmpty(appInsightsConnectionString))
            {
                // This service will collect and send telemetry data to Azure Monitor.
                services.AddOpenTelemetry().UseAzureMonitor(options =>
                {
                    options.ConnectionString = appInsightsConnectionString;
                });
            }
        }
    }
}