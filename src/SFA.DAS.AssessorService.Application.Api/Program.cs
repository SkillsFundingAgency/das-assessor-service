using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                var host = BuildWebHost(args);

                host.Run();

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
                .UseKestrel()
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}