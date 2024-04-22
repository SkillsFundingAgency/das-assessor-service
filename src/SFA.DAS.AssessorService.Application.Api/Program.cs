using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Application.Api.StartupConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.AssessorService.Application.Api
{
    using global::NLog.Web;
    using Microsoft.Extensions.Hosting;
    using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var logger = NLogBuilder.ConfigureNLog(environment == "Development" ? "nlog.Development.config" : "nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(
                    services =>
                    {
                        services.AddApplicationInsightsTelemetry();
                    })
                .UseStartup<Startup>()
                .UseNServiceBusContainer()
                .UseNLog();
        }

    }
}