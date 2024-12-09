using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.Orchestrators.Search;
using SFA.DAS.AssessorService.Web.Services;
using SFA.DAS.GovUK.Auth.AppStart;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Web.Extensions;
using System.Linq;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationServices(
            this IServiceCollection services,
            IWebConfiguration configuration,
            IConfiguration config)
        {
            services.AddSingleton<IWebConfiguration>(configuration);
            services.AddSingleton<IAzureApiClientConfiguration>(configuration.AzureApiAuthentication);
            services.AddSingleton<AssessorApiClientConfiguration>(configuration.AssessorApiAuthentication);
            services.AddSingleton<QnaApiClientConfiguration>(configuration.QnaApiAuthentication);
            services.Configure<AzureApiClientConfiguration>(config.GetSection("AzureApiAuthentication"));

            return services;
        }

        public static IServiceCollection AddLocalizationConfiguration(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(
            this IServiceCollection services,
            IConfiguration config,
            IWebConfiguration configuration)
        {
            var isLocal = string.IsNullOrEmpty(config["ResourceEnvironmentName"]) ||
                          config["ResourceEnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);

            var cookieDomain = isLocal ? "" : configuration.ServiceLink.Replace("https://", "", StringComparison.CurrentCultureIgnoreCase);
            var loginRedirect = isLocal ? "" : $"{configuration.ServiceLink}/service/account-details";

            services.AddAndConfigureGovUkAuthentication(config, 
                new AuthRedirects
                { 
                    CookieDomain = cookieDomain,
                    LoginRedirect = loginRedirect,
                    SignedOutRedirectUrl = "/account/signedout"

                },
                typeof(AssessorServiceAccountPostAuthenticationClaimsHandler));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.IsAuthenticated, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddTransient<ICustomClaims, AssessorServiceAccountPostAuthenticationClaimsHandler>();
            services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();

            services.AddSingleton<IAuthorizationPolicyProvider, AssessorPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, ApplicationAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PrivilegeAuthorizationHandler>();

            return services;
        }

        public static IServiceCollection AddDataProtectionConfiguration(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IWebConfiguration configuration,
            ILogger logger)
        {
            if (env.IsDevelopment())
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
                    var redisConnectionString = configuration.SessionRedisConnectionString;
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
                    logger.LogError(e, $"Error setting redis for session. Conn: {configuration.SessionRedisConnectionString}");
                    throw;
                }
            }

            return services;
        }

        public static IServiceCollection AddSessionConfiguration(this IServiceCollection services)
        {
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromHours(1);
                opt.Cookie = new CookieBuilder
                {
                    Name = ".Assessors.Session",
                    HttpOnly = true
                };
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddControllersWithViews(options => { options.Filters.Add<CheckSessionFilter>(); })
                .AddSessionStateTempDataProvider()
                .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<Startup>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("SFA"))
                    .ToArray()
            ));

            services.AddSingleton<Microsoft.AspNetCore.Mvc.ViewFeatures.IHtmlGenerator, CacheOverrideHtmlGenerator>();

            services.AddAntiforgery(options =>
                options.Cookie = new CookieBuilder
                {
                    Name = ".Assessors.AntiForgery",
                    HttpOnly = true
                });

            services.Configure<MvcViewOptions>(options =>
            {
                options.HtmlHelperOptions.CheckBoxHiddenInputRenderMode = CheckBoxHiddenInputRenderMode.None;
            });
            return services;

        }

        public static IServiceCollection AddApiClients(this IServiceCollection services) {
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
            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services, string environmentName, AzureApiClientConfiguration azureApiClientConfiguration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ISessionService>(sp => new SessionService(
                sp.GetRequiredService<IHttpContextAccessor>(),
                environmentName));
            services.AddTransient<IClaimService, ClaimService>();
            services.AddScoped<IOppFinderSession, OppFinderSession>();
            services.AddScoped<ICertificateHistorySession, CertificateHistorySession>();
            services.AddTransient<IAzureTokenService>(sp => new AzureTokenService(azureApiClientConfiguration));
            services.AddTransient<IApiValidationService, ApiValidationService>();
            services.AddSingleton<IDateTimeHelper, DateTimeHelper>();

            services.AddTransient<ILoginOrchestrator, LoginOrchestrator>();
            services.AddTransient<ISearchOrchestrator, SearchOrchestrator>();

            services.AddTransient<IApplicationService, ApplicationService>();
            return services;
        }

        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
            services.AddTransient<IQnaApiClientFactory, QnaApiClientFactory>();
            return services;
        }
    }
}
