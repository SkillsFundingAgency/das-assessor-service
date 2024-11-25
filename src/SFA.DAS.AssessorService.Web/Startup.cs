using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Azure;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.Services;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.Utils;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SFA.DAS.AssessorService.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, ILogger<Startup> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
            if (!configuration.IsDev())
            {
                config.AddJsonFile("appsettings.json", false)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif

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
            Configuration = _config.Get<WebConfiguration>();
        }

        private IWebConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMappings();
#if DEBUG
                try
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var mapper = serviceProvider.GetRequiredService<IMapper>();
                    mapper.ConfigurationProvider.AssertConfigurationIsValid();
                }
                catch (AutoMapperConfigurationException ex)
                {
                    _logger.LogError("AutoMapper configuration validation failed: {Message}", ex.Message);
                    throw; // Rethrow to prevent startup if configuration is invalid
                }
#endif

                services.AddApplicationInsightsTelemetry();

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.AddTransient<ICustomClaims, AssessorServiceAccountPostAuthenticationClaimsHandler>();
                services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();

                var isLocal = string.IsNullOrEmpty(_config["ResourceEnvironmentName"])
                                || _config["ResourceEnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
                var cookieDomain = isLocal ? "" : Configuration.ServiceLink.Replace("https://", "", StringComparison.CurrentCultureIgnoreCase);
                var loginRedirect = isLocal ? "" : $"{Configuration.ServiceLink}/service/account-details";
                services.AddAndConfigureGovUkAuthentication(_config, typeof(AssessorServiceAccountPostAuthenticationClaimsHandler), "/account/signedout", "/service/account-details", cookieDomain, loginRedirect);

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        PolicyNames.IsAuthenticated, policy =>
                        {
                            policy.RequireAuthenticatedUser();
                        });
                });


                services.Configure<RequestLocalizationOptions>(options =>
                {
                    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                    options.RequestCultureProviders.Clear();
                });

                services.AddSingleton<IAuthorizationPolicyProvider, AssessorPolicyProvider>();

                services.AddSingleton<IAuthorizationHandler, ApplicationAuthorizationHandler>();
                services.AddSingleton<IAuthorizationHandler, PrivilegeAuthorizationHandler>();

                services.Configure<MvcViewOptions>(options =>
                {
                    // Disable hidden checkboxes
                    options.HtmlHelperOptions.CheckBoxHiddenInputRenderMode = CheckBoxHiddenInputRenderMode.None;
                });

                services.AddControllersWithViews(options => { options.Filters.Add<CheckSessionFilter>(); })
                    .AddSessionStateTempDataProvider()
                    .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
                services.AddValidatorsFromAssemblyContaining<Startup>();

                services.AddSingleton<Microsoft.AspNetCore.Mvc.ViewFeatures.IHtmlGenerator, CacheOverrideHtmlGenerator>();

                services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".Assessors.AntiForgery", HttpOnly = true });

                if (_env.IsDevelopment())
                {
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "keys")))
                        .SetApplicationName("AssessorApply");

                    services.AddDistributedMemoryCache();
                }
                else
                {
                    try
                    {
                        var redisConnectionString = Configuration.SessionRedisConnectionString;
                        var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},DefaultDatabase=1");

                        services.AddDataProtection()
                            .PersistKeysToStackExchangeRedis(redis, "AssessorApply-DataProtectionKeys")
                            .SetApplicationName("AssessorApply");

                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = $"{redisConnectionString},DefaultDatabase=0";
                        });

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"Error setting redis for session.  Conn: {Configuration.SessionRedisConnectionString}");
                        throw;
                    }
                }

                services.AddSession(opt =>
                {
                    opt.IdleTimeout = TimeSpan.FromHours(1);
                    opt.Cookie = new CookieBuilder()
                    {
                        Name = ".Assessors.Session",
                        HttpOnly = true
                    };
                });

                services.AddHealthChecks();

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName.StartsWith("SFA"))
                        .ToArray()
                ));

                //Configuration
                services.AddSingleton<IWebConfiguration>(Configuration);
                services.AddSingleton<IAzureApiClientConfiguration>(Configuration.AzureApiAuthentication);
                services.AddSingleton<AssessorApiClientConfiguration>(Configuration.AssessorApiAuthentication);
                services.AddSingleton<QnaApiClientConfiguration>(Configuration.QnaApiAuthentication);
                services.Configure<AzureApiClientConfiguration>(_config.GetSection("AzureApiAuthentication"));


                ////Factories
                services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
                services.AddTransient<IQnaApiClientFactory, QnaApiClientFactory>();

                //ApiClients
                services.AddTransient<IAzureApiClient>(sp =>
                {
                    var tokenService = sp.GetRequiredService<IAzureTokenService>();
                    var logger = sp.GetRequiredService<ILogger<AzureApiClientBase>>();
                    var azureApiClientConfig = sp.GetRequiredService<IAzureApiClientConfiguration>();
                    var organisationsApiClient = sp.GetRequiredService<IOrganisationsApiClient>();
                    var contactsApiClient = sp.GetRequiredService<IContactsApiClient>();
                    var baseUri = azureApiClientConfig.ApiBaseAddress;

                    return new AzureApiClient(baseUri, tokenService, logger, azureApiClientConfig, organisationsApiClient, contactsApiClient);
                });

                services.AddTransient<IApplicationApiClient, ApplicationApiClient>();
                services.AddTransient<IApprovalsLearnerApiClient, ApprovalsLearnerApiClient>();
                services.AddTransient<ICertificateApiClient, CertificateApiClient>();
                services.AddTransient<IContactsApiClient, ContactsApiClient>();
                services.AddTransient<IDashboardApiClient, DashboardApiClient>();
                services.AddTransient<IEmailApiClient, EmailApiClient>();
                services.AddTransient<ILearnerDetailsApiClient, LearnerDetailsApiClient>();
                services.AddTransient<ILocationsApiClient, LocationsApiClient>();
                services.AddTransient<ILoginApiClient, LoginApiClient>();
                services.AddTransient<IOppFinderApiClient, OppFinderApiClient>();
                services.AddTransient<IOrganisationsApiClient, OrganisationsApiClient>();
                services.AddTransient<IQnaApiClient, QnaApiClient>();
                services.AddTransient<IRegisterApiClient, RegisterApiClient>();
                services.AddTransient<ISearchApiClient, SearchApiClient>();
                services.AddTransient<IStandardVersionApiClient, StandardVersionApiClient>();
                services.AddTransient<IStandardsApiClient, StandardsApiClient>();
                services.AddTransient<IValidationApiClient, ValidationApiClient>();


                ////Services
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddTransient<ISessionService>(sp => new SessionService(
                    sp.GetRequiredService<IHttpContextAccessor>(),
                    _env.EnvironmentName));
                services.AddTransient<IClaimService, ClaimService>();
                services.AddScoped<IOppFinderSession, OppFinderSession>();
                services.AddScoped<ICertificateHistorySession, CertificateHistorySession>();
                services.AddTransient<IAzureTokenService>(sp => new AzureTokenService(Configuration.AzureApiAuthentication));
                services.AddTransient<IApiValidationService, ApiValidationService>();
                services.AddSingleton<IDateTimeHelper, DateTimeHelper>();

                services.AddTransient<ILoginOrchestrator, LoginOrchestrator>();
                services.AddTransient<ISearchOrchestrator, SearchOrchestrator>();

                services.AddTransient<IApplicationService, ApplicationService>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Startup Configure Services");
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection()
                .UseSecurityHeaders()
                .UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseRequestLocalization()
                .UseHealthChecks("/health")
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" });
                });
        }

    }
}
