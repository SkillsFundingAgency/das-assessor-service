using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.Utils;
using SFA.DAS.Configuration.AzureTableStorage;
using System;
using System.IO;

namespace SFA.DAS.AssessorService.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, ILogger<Startup> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif

            config.AddEnvironmentVariables();
            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            }
            );

            _config = config.Build();
            Configuration = _config.Get<WebConfiguration>();
        }

        private IWebConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMappings();

                services.AddConfigurationServices(Configuration, _config)
                    .AddLocalizationConfiguration()
                    .AddAuthenticationAndAuthorization(_config, Configuration)
                    .AddApplicationServices()
                    .AddDataProtectionConfiguration(_env, Configuration, _logger)
                    .AddSessionConfiguration();

                services.AddHealthChecks();

                services.AddFactories()
                    .AddApiClients()
                    .AddCustomServices(_env.EnvironmentName, Configuration.AzureApiAuthentication);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Startup Configure Services");
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection()
                .UseSecurityHeaders()
                .UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseRequestLocalization()
                .UseHealthChecks("/health")
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" });
                });
        }

    }
}
