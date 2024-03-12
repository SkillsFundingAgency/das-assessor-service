using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CharityCommission;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CompaniesHouse;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using StructureMap;
using Swashbuckle.AspNetCore.Filters;
using static CharityCommissionService.SearchCharitiesV1SoapClient;

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

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Configuration = ConfigurationService
                .GetConfigApi(_config["EnvironmentName"], _config["ConfigurationStorageConnectionString"], VERSION, SERVICE_NAME).Result;

            IServiceProvider serviceProvider;
            try
            {
                services.AddAuthentication(o =>
                    {
                        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(o =>
                    {
                        var validAudiences = new List<string>();
                        var authority = string.Empty;

                        if (_useSandbox)
                        {
                            validAudiences.AddRange(Configuration.SandboxApiAuthentication.Audiences.Split(","));
                            authority = o.Authority = Configuration.SandboxApiAuthentication.Tenant;
                        }
                        else
                        {
                            validAudiences.AddRange(Configuration.ApiAuthentication.Audiences.Split(","));
                            authority = o.Authority = Configuration.ApiAuthentication.Tenant;
                        }

                        o.Authority = $"https://login.microsoftonline.com/{authority}";
                        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                            ValidAudiences = validAudiences
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

                services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

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
                    .AddNewtonsoftJson();

                services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
                services.AddValidatorsFromAssemblyContaining<Startup>();

                services.AddSwaggerGen(config =>
                    {
                        config.SwaggerDoc("v1", new OpenApiInfo { Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1" });
                        config.CustomSchemaIds(i => i.FullName);
                        
                        if (_env.IsDevelopment())
                        {
                            var basePath = AppContext.BaseDirectory;
                            var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                            config.IncludeXmlComments(xmlPath);
                        }

                        if (!_env.IsDevelopment())
                        {
                            config.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                            {
                                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                                In = ParameterLocation.Header,
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey
                            });

                            config.OperationFilter<SecurityRequirementsOperationFilter>();
                        }
                    });

                services.AddHttpClient<CompaniesHouseApiClient>("CompaniesHouseApiClient", config =>
                    {
                        config.BaseAddress = new Uri(Configuration.CompaniesHouseApiAuthentication.ApiBaseAddress); //  "https://api.companieshouse.gov.uk"
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHttpClient<OuterApiClient>().SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHostedService<TaskQueueHostedService>();

                services.AddHealthChecks();

                serviceProvider = ConfigureIOC(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Startup Configure Services");
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
                    _.AssembliesFromApplicationBaseDirectory(c => c.FullName.StartsWith("SFA"));
                    _.WithDefaultConventions();

                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    _.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                config.For<IApiConfiguration>().Use(Configuration);
                config.For<ServiceFactory>().Use<ServiceFactory>(ctx => t => ctx.GetInstance(t));
                config.For<IMediator>().Use<Mediator>();
                config.For<ISignInService>().Use<SignInService>();
              
                var sqlConnectionString = _useSandbox ? Configuration.SandboxSqlConnectionString : Configuration.SqlConnectionString;
                config.AddDatabaseRegistration(Configuration.Environment, sqlConnectionString);

                config.For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(string.IsNullOrWhiteSpace(NotificationConfiguration().ClientId)
                    ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(NotificationConfiguration())).Build()
                    : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(NotificationConfiguration())).Build());

                config.For<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>().Use(NotificationConfiguration());

                config.For<RoatpApiClientConfiguration>().Use(Configuration.RoatpApiAuthentication);
                config.For<QnaApiClientConfiguration>().Use(Configuration.QnaApiAuthentication);
                config.For<ReferenceDataApiClientConfiguration>().Use(Configuration.ReferenceDataApiAuthentication);

                config.ForSingletonOf<IBackgroundTaskQueue>().Use<BackgroundTaskQueue>();

                // This is a SOAP service. The client interfaces are contained within the generated proxy code
                config.For<CharityCommissionService.ISearchCharitiesV1SoapClient>().Use<CharityCommissionService.SearchCharitiesV1SoapClient>()
                    .Ctor<EndpointConfiguration>().Is(EndpointConfiguration.SearchCharitiesV1Soap);

                config.For<ICharityCommissionApiClient>().Use<CharityCommissionApiClient>()
                    .Ctor<ICharityCommissionApiClientConfiguration>().Is(Configuration.CharityCommissionApiAuthentication);

                config.For<ICompaniesHouseApiClient>().Use<CompaniesHouseApiClient>()
                    .Ctor<ICompaniesHouseApiClientConfiguration>().Is(Configuration.CompaniesHouseApiAuthentication);

                config.For<IOuterApiClient>().Use<OuterApiClient>()
                    .Ctor<IOuterApiClientConfiguration>().Is(Configuration.OuterApi);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

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
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api v1");
                    })
                    .UseAuthentication();

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
