using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.StartupConfiguration;
using SFA.DAS.Telemetry.Startup;

namespace SFA.DAS.AssessorService.Application.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddOpenTelemetryRegistration(context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!)
                        .AddTelemetryUriRedaction("dob,name");
                })
                .UseStartup<Startup>();
        }
    }
}