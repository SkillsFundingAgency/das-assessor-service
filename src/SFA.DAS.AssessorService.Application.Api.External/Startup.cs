﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration;
using SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers;
using SFA.DAS.AssessorService.Settings;
using StructureMap;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SFA.DAS.AssessorService.Application.Api.External
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";
        private readonly bool UseSandbox;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before Config");
            Configuration = configuration;

            if(!bool.TryParse(configuration["UseSandboxServices"], out UseSandbox))
            {
                UseSandbox = "yes".Equals(configuration["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {UseSandbox.ToString()}");
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

                if (UseSandbox)
                {
                    services.AddHttpClient<IApiClient, SandboxApiClient>(config =>
                    {
                        config.BaseAddress = new Uri(ApplicationConfiguration.SandboxClientApiAuthentication.ApiBaseAddress);
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                        config.Timeout = TimeSpan.FromMinutes(5);
                    });
                }
                else
                {
                    services.AddHttpClient<IApiClient, ApiClient>(config =>
                    {
                        config.BaseAddress = new Uri(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                        config.Timeout = TimeSpan.FromMinutes(5);
                    });
                }

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = $"Assessor Service API {Configuration["InstanceName"]}", Version = "v1" });
                    c.EnableAnnotations();
                    c.OperationFilter<UpdateOptionalParamatersWithDefaultValues>();
                    c.OperationFilter<ExamplesOperationFilter>();
                    c.SchemaFilter<NullableSchemaFilter>();
                    c.SchemaFilter<SwaggerRequiredSchemaFilter>();
                    c.CustomSchemaIds(x => x.FullName.Replace("SFA.DAS.AssessorService.Application.Api.External.Models.", ""));

                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.External.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });

                services.AddScoped<IHeaderInfo, HeaderInfo>();
                services.AddHttpContextAccessor();

                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    });

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.Configure<RequestLocalizationOptions>(
                    options =>
                    {
                        options.DefaultRequestCulture = new RequestCulture("en-GB");
                        options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                        options.RequestCultureProviders.Clear();
                    });

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
                MappingStartup.AddMappings();

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

                if (UseSandbox)
                {
                    app.UseMiddleware<SandboxHeadersMiddleware>();
                }

                app.UseMiddleware<GetHeadersMiddleware>();

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
