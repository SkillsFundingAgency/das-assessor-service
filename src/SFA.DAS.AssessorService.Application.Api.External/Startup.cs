using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration;
using SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Configuration.AzureTableStorage;
using StructureMap;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.External
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private readonly bool _useSandbox;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
            _logger.LogInformation("In startup constructor.  Before Config");

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
            Configuration = _config.Get<ExternalApiConfiguration>();

            if(!bool.TryParse(configuration["UseSandboxServices"], out _useSandbox))
            {
                _useSandbox = "yes".Equals(configuration["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {_useSandbox.ToString()}");
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        private IExternalApiConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Assessor Service API {_config["InstanceName"]}", Version = "v1" });
                    c.EnableAnnotations();
                    c.ExampleFilters();
                    c.SchemaFilter<SwaggerRequiredSchemaFilter>();
                    c.CustomSchemaIds(x => x.FullName.Replace("SFA.DAS.AssessorService.Application.Api.External.Models.", ""));

                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.External.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });
                services.AddSwaggerExamplesFromAssemblyOf<Startup>();

                services.AddScoped<IHeaderInfo, HeaderInfo>();
                services.AddHttpContextAccessor();

                services.AddMvc()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            try
                            {
                                var requestUrl = context.HttpContext.Request.Path;
                                var requestMethod = context.HttpContext.Request.Method;
                                var modelErrors = context.ModelState.SelectMany(model => model.Value.Errors.Select(err => err.ErrorMessage));
                                _logger.LogError($"Invalid request detected. {requestMethod.ToUpper()}: {requestUrl} - Errors: {string.Join(",", modelErrors)}");
                            }
                            catch
                            {
                                // safe to ignore!
                            }

                            var error = new ApiResponse((int)HttpStatusCode.Forbidden, "Your request contains invalid input. Please ensure it matches the swagger definition and try again.");
                            return new BadRequestObjectResult(error) { StatusCode = error.StatusCode };
                        };
                    }
                    )
                    .AddNewtonsoftJson(options =>
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

            container.Configure((Action<ConfigurationExpression>)(config =>
            {
                config.Scan(_ =>
                {
                    _.AssembliesFromApplicationBaseDirectory(c => c.FullName.StartsWith("SFA"));
                    _.WithDefaultConventions();
                });

                if (_useSandbox)
                {
                    config.For<AssessorApiClientConfiguration>().Use(Configuration.SandboxAssessorApiAuthentication);
                    config.For<IApiClient>().Use<SandboxApiClient>();
                }
                else
                {
                    config.For<AssessorApiClientConfiguration>().Use(Configuration.AssessorApiAuthentication);
                    config.For<IApiClient>().Use<ApiClient>();
                }

                config.For<IExternalApiConfiguration>().Use(Configuration);

                config.Populate(services);
            }));

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

                if (_useSandbox)
                {
                    app.UseMiddleware<SandboxHeadersMiddleware>();
                }

                app.UseMiddleware<GetHeadersMiddleware>();

                app.UseHttpsRedirection();
                app.UseSecurityHeaders();

                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
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
