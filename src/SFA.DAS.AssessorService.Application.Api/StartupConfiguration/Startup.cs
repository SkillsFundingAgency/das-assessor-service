using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SFA.DAS.AssessorService.Application.Api.Extensions;

namespace SFA.DAS.AssessorService.Application.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Data;
    using Data.TestData;
    using ExternalApis.AssessmentOrgs;
    using FluentValidation.AspNetCore;
    using JWT;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Settings;
    using StartupConfiguration;
    using StructureMap;
    using Swashbuckle.AspNetCore.Swagger;

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
            Configuration = ConfigurationService
                .GetConfig(config["EnvironmentName"], config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;
        }

        public IWebConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddAndConfigureAuthentication(Configuration);

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
                c.SwaggerDoc("v1", new Info {Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1"});

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

            var serviceProvider = ConfigureIOC(services);

            TestDataService.AddTestData(serviceProvider.GetService<AssessorDbContext>());

            ValidatorOptions.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

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
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(null));
                config.For<IDateTimeProvider>().Use<UtcDateTimeProvider>();

                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(Configuration.SqlConnectionString);

                config.For<AssessorDbContext>().Use(c => new AssessorDbContext(option.Options, _env.IsDevelopment()));

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            MappingStartup.AddMappings();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api v1");
                })
                .UseAuthentication()
                .UseMiddleware(typeof(ErrorHandlingMiddleware))
                .UseMvc();
        }
    }
}
