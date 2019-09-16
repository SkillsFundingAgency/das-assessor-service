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
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
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
        private readonly bool UseSandbox;

        public Startup(IHostingEnvironment env, IConfiguration config, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before GetConfig");
            Configuration = ConfigurationService
                .GetConfig(config["EnvironmentName"], config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            if (!bool.TryParse(config["UseSandboxServices"], out UseSandbox))
            {
                UseSandbox = "yes".Equals(config["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {UseSandbox.ToString()}");
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
                                UseSandbox ? Configuration.SandboxApiAuthentication.Audience : Configuration.ApiAuthentication.Audience,
                                UseSandbox ? Configuration.SandboxApiAuthentication.ClientId : Configuration.ApiAuthentication.ClientId
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
                    c.CustomSchemaIds(i => i.FullName);
                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });

                services.AddHttpClient<ProviderRegisterApiClient>("ProviderRegisterApiClient", config =>
                    {
                        config.BaseAddress = new Uri(Configuration.ProviderRegisterApiAuthentication.ApiBaseAddress); //  "https://findapprenticeshiptraining-api.sfa.bis.gov.uk"
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHttpClient<ReferenceDataApiClient>("ReferenceDataApiClient", config =>
                    {
                        config.BaseAddress = new Uri(Configuration.ReferenceDataApiAuthentication.ApiBaseAddress); //  "https://at-refdata.apprenticeships.sfa.bis.gov.uk/api"
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHttpClient<CompaniesHouseApiClient>("CompaniesHouseApiClient", config =>
                    {
                        config.BaseAddress = new Uri(Configuration.CompaniesHouseApiAuthentication.ApiBaseAddress); //  "https://api.companieshouse.gov.uk"
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHealthChecks();

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
                config.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                config.For<IMediator>().Use<Mediator>();
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(Configuration.AssessmentOrgsApiClientBaseUrl));  
                config.For<IIfaStandardsApiClient>().Use(() => new IfaStandardsApiClient(Configuration.IfaApiClientBaseUrl));
          
                config.For<IDateTimeProvider>().Use<UtcDateTimeProvider>();
                config.For<ISignInService>().Use<SignInService>();
              
                var sqlConnectionString = UseSandbox ? Configuration.SandboxSqlConnectionString : Configuration.SqlConnectionString;
                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(sqlConnectionString, options => options.EnableRetryOnFailure(3));

                config.For<AssessorDbContext>().Use(c => new AssessorDbContext(option.Options));
                config.For<IDbConnection>().Use(c => new SqlConnection(sqlConnectionString));


                config.For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(string.IsNullOrWhiteSpace(NotificationConfiguration().ClientId)
                    ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(NotificationConfiguration())).Build()
                    : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(NotificationConfiguration())).Build());

                config.For<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>().Use(NotificationConfiguration());

                // NOTE: These are SOAP Services. Their client interfaces are contained within the generated Proxy code.
                config.For<CharityCommissionService.ISearchCharitiesV1SoapClient>().Use<CharityCommissionService.SearchCharitiesV1SoapClient>();
                config.For<CharityCommissionApiClient>().Use<CharityCommissionApiClient>();
                // End of SOAP Services

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                MappingStartup.AddMappings();

                //app.UseSecurityHeaders();
                
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
                    })
                    .UseAuthentication();

                app.UseMiddleware(typeof(ErrorHandlingMiddleware));
                
                app.UseRequestLocalization();
                app.UseHealthChecks("/health");
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
