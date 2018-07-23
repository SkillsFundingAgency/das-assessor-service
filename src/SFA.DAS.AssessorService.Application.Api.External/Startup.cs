using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Settings;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.AssessorService.Application.Api.External
{
    //TODO: This all needs configuring correctly as I don't know what I'm supposed to put in here
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

                services.AddHttpClient<ApiClient>("ApiClient", config =>
                {
                    config.BaseAddress = new Uri(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                    config.DefaultRequestHeaders.Add("Accept", "Application/json");
                });

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "SFA.DAS.AssessorService.Application.Api.External", Version = "v1" });

                //if (_env.IsDevelopment())
                //{
                //    var basePath = AppContext.BaseDirectory;
                //    var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.External.xml");
                //    c.IncludeXmlComments(xmlPath);
                //}
            });

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                return ConfigureIoC(services);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure Services");
                throw;
            }
        }

        private IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                });

                config.For<ITokenService>().Use<TokenService>();
                config.For<IWebConfiguration>().Use(ApplicationConfiguration);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api.External v1");
                    })
                    .UseAuthentication();

                app.UseHttpsRedirection();
                app.UseMvc();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure");
                throw;
            }
        }
    }
}
