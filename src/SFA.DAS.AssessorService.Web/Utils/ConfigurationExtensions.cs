using System;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.AssessorService.Web.Utils;

public static class ConfigurationExtensions
{
    public static bool IsDev(this IConfiguration configuration)
    {
        return configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
    public static bool IsLocal(this IConfiguration configuration)
    {
        return configuration["Environment"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
    }
}