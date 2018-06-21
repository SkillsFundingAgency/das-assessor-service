using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace SFA.DAS.AssessorService.Web.Staff
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

        public static IWebHost BuildWebHost(string[] args)
        { 
            IHostingEnvironment hostingEnvironment = null;

            return  WebHost.CreateDefaultBuilder(args)
            .UseApplicationInsights()
            .ConfigureServices(
                services =>
                {
                    hostingEnvironment = services
                        .Where(x => x.ServiceType == typeof(IHostingEnvironment))
                        .Select(x => (IHostingEnvironment) x.ImplementationInstance)
                        .First();
                })
            .UseKestrel(options =>
            {
                options.AddServerHeader = false;
                if (hostingEnvironment.IsDevelopment())
                {
                    options.Listen(IPAddress.Loopback, 44347, listenOptions =>
                    {
                        listenOptions.UseHttps("sfa.das.assessorservice.pfx", "C0ventry18");
                    });
                }
            })
            .UseStartup<Startup>()
            .UseNLog()
            .Build();
        }
           
    }
}
