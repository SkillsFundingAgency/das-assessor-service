using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static bool IsLocalAcceptanceOrDev(this IApiConfiguration config)
        {
            return config.Environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   config.Environment.Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase) ||
                   config.Environment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsIntegrationTests(this IApiConfiguration config)
        {
            return config.Environment.Equals("IntegrationTests", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
