using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration;
using SFA.DAS.AssessorService.Settings;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string SERVICE_NAME = "SFA.DAS.AssessorService.ExternalApi";
        private const string VERSION = "1.0";
        private readonly bool _useSandbox;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before Config");
            Configuration = configuration;

            if (!bool.TryParse(configuration["UseSandboxServices"], out _useSandbox))
            {
                _useSandbox = "yes".Equals(configuration["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {_useSandbox.ToString()}");
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        public IConfiguration Configuration { get; }
        public IExternalApiConfiguration ApplicationConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                ApplicationConfiguration = ConfigurationService.GetConfigExternalApi(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], VERSION, SERVICE_NAME).Result;

                services.AddCustomAuthenticationAndAuthorization(Configuration, _useSandbox, ApplicationConfiguration);

                services.AddCustomControllers(_logger)
                    .AddCustomLocalization()
                    .AddCustomSwagger(Configuration["InstanceName"], _env)
                    .AddApiClients(_useSandbox, ApplicationConfiguration)
                    .AddHttpContextServices();

                services.AddMappings();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure Services");
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assessor Service API v1");
                    })
                    .UseAuthentication();

                if (_useSandbox)
                {
                    app.UseMiddleware<SandboxHeadersMiddleware>();
                }

                app.UseMiddleware<GetHeadersMiddleware>();

                app.UseHttpsRedirection();
                app.UseSecurityHeaders();

                app.UseRouting();
                app.UseAuthorization();
                app.UseAuthentication();
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
    }
}
