using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Listen(IPAddress.Loopback, 5015, listenOptions =>
                    {
                        listenOptions.UseHttps("sfa.das.assessorservice.pfx", "C0ventry18");
                    });
                })
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}
