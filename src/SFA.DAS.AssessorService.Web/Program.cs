using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IWebHostEnvironment hostingEnvironment = null;

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                    {
                        hostingEnvironment = services
                            .Where(x => x.ServiceType == typeof(IWebHostEnvironment))
                            .Select(x => (IWebHostEnvironment)x.ImplementationInstance)
                            .First();
                        services.AddOpenTelemetryRegistration(context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!);
                    })
                .UseStartup<Startup>();
        }
    }
}
