using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Application.Api.StartupConfiguration;

namespace SFA.DAS.AssessorService.Application.Api
{
    using global::NLog.Web;

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
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseKestrel()
                .UseNLog();
        }
    }
}