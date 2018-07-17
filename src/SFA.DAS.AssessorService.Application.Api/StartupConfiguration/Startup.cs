using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using FluentValidation.AspNetCore;
using JWT;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Data.TestData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Settings;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public class Startup
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public Startup(IHostingEnvironment env, IConfiguration config, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before GetConfig");
            Configuration = ConfigurationService
                .GetConfig(config["EnvironmentName"], config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        public IWebConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IServiceProvider serviceProvider;
            //services.AddAndConfigureAuthentication(Configuration);
            try
            {
                //services.AddApplicationInsightsTelemetry();
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {                                     
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });

                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddAzureAdBearer(options =>
                {
                    options.ClientId = Configuration.ApiAuthentication.ClientId;
                    options.Instance = Configuration.ApiAuthentication.Instance;
                    options.TenantId = Configuration.ApiAuthentication.TenantId;
                    options.Audience = Configuration.ApiAuthentication.Audience;
                });

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                IMvcBuilder mvcBuilder;
                if (_env.IsDevelopment())
                    mvcBuilder = services.AddMvc(opt => { opt.Filters.Add(new AllowAnonymousFilter()); });
                else
                    mvcBuilder = services.AddMvc();

                mvcBuilder
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                        opts => { opts.ResourcesPath = "Resources"; })
                    .AddDataAnnotationsLocalization()
                    .AddControllersAsServices()
                    .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1" });

                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });

                services.Configure<RequestLocalizationOptions>(
                    opts =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                        new CultureInfo("en-GB")
                        };

                        opts.DefaultRequestCulture = new RequestCulture("en-GB");
                        opts.SupportedCultures = supportedCultures;
                        opts.SupportedUICultures = supportedCultures;
                    });

                serviceProvider = ConfigureIOC(services);

                if (_env.IsDevelopment())
                {
                    TestDataService.AddTestData(serviceProvider.GetService<AssessorDbContext>());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure Services");
                throw;
            }

            return serviceProvider;
        }

        private IServiceProvider ConfigureIOC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    //_.AssemblyContainingType(typeof(Startup));
                    _.AssembliesFromApplicationBaseDirectory(c => c.FullName.StartsWith("SFA"));
                    _.WithDefaultConventions();

                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    _.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                config.For<IWebConfiguration>().Use(Configuration);
                config.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                config.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                config.For<IMediator>().Use<Mediator>();
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(Configuration.AssessmentOrgsApiClientBaseUrl));
                config.For<IDateTimeProvider>().Use<UtcDateTimeProvider>();

                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(Configuration.SqlConnectionString);

                config.For<AssessorDbContext>().Use(c => new AssessorDbContext(option.Options));

                config.For<IDbConnection>().Use(c => new SqlConnection(Configuration.SqlConnectionString));

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {

                MappingStartup.AddMappings();

                if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api v1");
                    })
                    .UseAuthentication();

                //TODO: put this back, but it's a bugger when coding.
                //app.UseMiddleware(typeof(ErrorHandlingMiddleware));
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
