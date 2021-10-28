using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;

namespace SFA.DAS.AssessorService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
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
            IWebHostEnvironment hostingEnvironment = null;

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(
                    services =>
                    {
                        hostingEnvironment = services
                            .Where(x => x.ServiceType == typeof(IWebHostEnvironment))
                            .Select(x => (IWebHostEnvironment) x.ImplementationInstance)
                            .First();
                        services.AddApplicationInsightsTelemetry();
                    })
                .UseStartup<Startup>()
                .UseUrls("https://localhost:5015")
                .UseNLog();
        }
    }
}
