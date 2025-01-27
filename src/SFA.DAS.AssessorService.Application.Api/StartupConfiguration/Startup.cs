using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public class Startup
    {
        private const string SERVICE_NAME = "SFA.DAS.AssessorService.Api";
        private const string VERSION = "1.0";
        
        private readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly bool _useSandbox;

        public Startup(IConfiguration config, ILogger<Startup> logger, IWebHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
            
            _logger.LogInformation("In startup constructor.  Before GetConfig");
            
            if (!bool.TryParse(config["UseSandboxServices"], out _useSandbox))
            {
                _useSandbox = "yes".Equals(config["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {_useSandbox.ToString()}");
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        private IApiConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMappings(_logger);

                Configuration = ConfigurationService
                    .GetConfigApi(_config["EnvironmentName"], _config["ConfigurationStorageConnectionString"], VERSION, SERVICE_NAME).Result;

                services.AddBaseConfiguration(Configuration);

                var notificationConfig = NotificationConfiguration();
                services.AddSingleton<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>(notificationConfig);
                services.AddSingleton<IJwtClientConfiguration>(sp =>
                    sp.GetRequiredService<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>());

                services.AddDatabaseRegistration(_useSandbox, Configuration);

                services
                    .AddCustomAuthentication(_useSandbox, Configuration)
                    .AddSwaggerDocumentation(_env)
                    .AddHttpAndApiClients(Configuration, notificationConfig)
                    .AddBackgroundServices()
                    .AddApplicationServices()
                    .AddDistributedMemoryCache()
                    .AddCustomLocalization()
                    .AddCustomControllers(_env)
                    .AddIisServerOptions();

                services.AddHealthChecks();

                services.AddCustomServices()
                    .AddHelpers()
                    .RegisterRepositories()
                    .RegisterValidators();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Startup Configure Services");
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {                
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api v1");
                    });

                app.UseMiddleware(typeof(ErrorHandlingMiddleware));
                
                app.UseRequestLocalization();
                app.UseHealthChecks("/health");
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    if (env.IsDevelopment())
                    {
                        endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                    }
                    else
                    {
                        endpoints.MapControllers();
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure");
                throw;
            }
        }

        private Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration NotificationConfiguration()
        {
            return new Notifications.Api.Client.Configuration.NotificationsApiClientConfiguration
            {
                ApiBaseUrl = Configuration.NotificationsApiClientConfiguration.ApiBaseUrl,
#pragma warning disable 618
                ClientToken = Configuration.NotificationsApiClientConfiguration.ClientToken,
#pragma warning restore 618
                ClientId = "",
                ClientSecret = "",
                IdentifierUri = "",
                Tenant = ""
            };
        }
    }
}
