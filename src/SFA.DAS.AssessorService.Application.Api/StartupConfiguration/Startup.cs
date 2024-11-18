using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
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
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CompaniesHouse;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMappings();

            Configuration = ConfigurationService
                .GetConfigApi(_config["EnvironmentName"], _config["ConfigurationStorageConnectionString"], VERSION, SERVICE_NAME).Result;

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

                services.AddControllers(options =>
                {
                    if (_env.IsDevelopment())
                    {
                        options.Filters.Add(new AllowAnonymousFilter());
                    }
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
                {
                    opts.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization()
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

                services.AddHttpClient<ICompaniesHouseApiClient, CompaniesHouseApiClient>(config =>
                    {
                        config.BaseAddress = new Uri(Configuration.CompaniesHouseApiAuthentication.ApiBaseAddress);
                        config.DefaultRequestHeaders.Add("Accept", "Application/json");
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHttpClient<IOuterApiClient, OuterApiClient>(config => 
                    {
                        config.BaseAddress = new Uri(Configuration.OuterApi.BaseUrl);
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHostedService<TaskQueueHostedService>();

                services.AddHealthChecks();
                services.AddTransient<IDateTimeHelper, DateTimeHelper>();

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName.StartsWith("SFA"))
                        .ToArray()
                ));

                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddTransient<JwtBearerTokenGenerator>();
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

                var sqlConnectionString = _useSandbox ? Configuration.SandboxSqlConnectionString : Configuration.SqlConnectionString;
                services.AddDatabaseRegistration(Configuration.Environment, sqlConnectionString);

                //Configuration
                services.AddSingleton<IApiConfiguration>(Configuration);
                services.AddSingleton<RoatpApiClientConfiguration>(Configuration.RoatpApiAuthentication);
                services.AddSingleton<QnaApiClientConfiguration>(Configuration.QnaApiAuthentication);
                services.AddSingleton<ReferenceDataApiClientConfiguration>(Configuration.ReferenceDataApiAuthentication);
                services.AddSingleton<ICharityCommissionApiClientConfiguration>(Configuration.CharityCommissionApiAuthentication);
                services.AddSingleton<ICompaniesHouseApiClientConfiguration>(Configuration.CompaniesHouseApiAuthentication);
                services.AddSingleton<IOuterApiClientConfiguration>(Configuration.OuterApi);

                var notificationConfig = NotificationConfiguration();
                services.AddSingleton<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>(notificationConfig);

                services.AddSingleton<IJwtClientConfiguration>(sp =>
                    sp.GetRequiredService<Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration>());

                services.AddApiClients(notificationConfig);
                services.AddCustomServices();

                services.RegisterRepositories();
                services.RegisterValidators();


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
