using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Extensions;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Data.TestData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
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
            try
            {            
                services.AddAuthentication(o =>
                    {
                        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(o =>
                    {
                        o.Authority = $"https://login.microsoftonline.com/{Configuration.ApiAuthentication.TenantId}"; 
                        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                            ValidAudiences = new List<string>
                            {
                                Configuration.ApiAuthentication.Audience,
                                Configuration.ApiAuthentication.ClientId
                            }
                        };
                        o.Events = new JwtBearerEvents()
                        {
                            OnTokenValidated = context => { return Task.FromResult(0); }
                        };
                    });    
                
                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
                
                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new RequestCulture("en-GB");
                    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                    options.RequestCultureProviders.Clear();
                });

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
                config.For<IIfaStandardsApiClient>().Use(() => new IfaStandardsApiClient(Configuration.IfaApiClientBaseUrl));
                config.For<IDateTimeProvider>().Use<UtcDateTimeProvider>();
                config.For<IDfeSignInService>().Use<DfeSignInService>();
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(Configuration.SqlConnectionString, options => options.EnableRetryOnFailure(3));

                config.For<AssessorDbContext>().Use(c => new AssessorDbContext(option.Options));
                config.For<IDbConnection>().Use(c => new SqlConnection(Configuration.SqlConnectionString));
              

                config.For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(string.IsNullOrWhiteSpace(NotificationConfiguration().ClientId)
                    ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(NotificationConfiguration())).Build()
                    : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(NotificationConfiguration())).Build());

                config.For<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>().Use(NotificationConfiguration());
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

                app.UseMiddleware(typeof(ErrorHandlingMiddleware));
                
                app.UseRequestLocalization();

                app.UseMvc();
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
                ClientToken = Configuration.NotificationsApiClientConfiguration.ClientToken,
                ClientId = "",
                ClientSecret = "",
                IdentifierUri = "",
                Tenant = ""
            };
        }
    
    }
}
