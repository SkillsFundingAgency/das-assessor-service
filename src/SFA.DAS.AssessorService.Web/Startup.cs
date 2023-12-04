using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.QnA;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Helpers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.Utils;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Services;
using StackExchange.Redis;
using StructureMap;
using NLog;
using System.Security.Claims;

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

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IServiceProvider serviceProvider;
            try
            {
                services.AddApplicationInsightsTelemetry();

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.AddTransient<ICustomClaims, AssessorServiceAccountPostAuthenticationClaimsHandler>();
                services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();
                if (Configuration.UseGovSignIn)
                {
                    var isLocal = string.IsNullOrEmpty(_config["ResourceEnvironmentName"]) 
                                  || _config["ResourceEnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
                    var cookieDomain = isLocal ? "" : Configuration.ServiceLink.Replace("https://","", StringComparison.CurrentCultureIgnoreCase);
                    var loginRedirect = isLocal ? "" : $"{Configuration.ServiceLink}/service/account-details";
                    services.AddAndConfigureGovUkAuthentication(_config, typeof(AssessorServiceAccountPostAuthenticationClaimsHandler), "/account/signedout","/service/account-details",cookieDomain, loginRedirect);   
                }
                else
                {
                    services.AddAndConfigureAuthentication(Configuration, _env);    
                }

                services.AddAuthorization();

                
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

                services.AddMvc(options => { options.Filters.Add<CheckSessionFilter>(); })
                    .AddControllersAsServices()
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
                        var redis = ConnectionMultiplexer.Connect(
                            $"{Configuration.SessionRedisConnectionString},DefaultDatabase=1");

                        services.AddDataProtection()
                            .PersistKeysToStackExchangeRedis(redis, "AssessorApply-DataProtectionKeys")
                            .SetApplicationName("AssessorApply");
                        services.AddDistributedRedisCache(options =>
                        {
                            options.Configuration = $"{Configuration.SessionRedisConnectionString},DefaultDatabase=0";
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

                services.AddHttpClient<IOrganisationsApiClient, OrganisationsApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IStandardsApiClient, StandardsApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IOppFinderApiClient, OppFinderApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IDashboardApiClient, DashboardApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IContactsApiClient, ContactsApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<ISearchApiClient, SearchApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IEmailApiClient, EmailApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IValidationApiClient, ValidationApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<ICertificateApiClient, CertificateApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<ILoginApiClient, LoginApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IApplicationApiClient, ApplicationApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<ILearnerDetailsApiClient, LearnerDetailApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IApprovalsLearnerApiClient, ApprovalsLearnerApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<ILocationsApiClient, LocationsApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });
                services.AddHttpClient<IStandardVersionClient, StandardVersionClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.AssessorApiAuthentication.ApiBaseAddress); });

                services.AddHttpClient<IQnaApiClient, QnaApiClient>(cfg => { cfg.BaseAddress = new Uri(Configuration.QnaApiAuthentication.ApiBaseAddress); });

                services.AddHttpClient<IRoatpApiClient, RoatpApiClient>(cfg =>
                {
                    cfg.BaseAddress = new Uri(Configuration.RoatpApiAuthentication.ApiBaseAddress); //  "https://at-providers-api.apprenticeships.education.gov.uk"
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

                services.AddHealthChecks();

                serviceProvider = ConfigureIoc(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Startup Configure Services");
                throw;
            }

            return serviceProvider;
        }

        private IServiceProvider ConfigureIoc(IServiceCollection services)
        {
            var container = new Container();
            var tokenLogger = services.BuildServiceProvider().GetService<ILogger<TokenService>>();
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                });

                config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);
                config.For<IOppFinderSession>().Use<OppFinderSession>();
                config.For<ICertificateHistorySession>().Use<CertificateHistorySession>();
                config.For<IWebConfiguration>().Use(Configuration);

                config.For<IAzureTokenService>().Use<AzureTokenService>();

                config.For<IAssessorTokenService>().Use<TokenService>()
                    .Ctor<IClientApiAuthentication>().Is(Configuration.AssessorApiAuthentication)
                    .Ctor<string>().Is(_config["EnvironmentName"]);

            //config.For<IQnATokenService>().Use<TokenService>()
            //    .Ctor<IClientApiAuthentication>().Is(Configuration.QnaApiAuthentication)
            //    .Ctor<string>().Is(_config["EnvironmentName"]);
            

                services.AddTransient<IQnATokenService, TokenService>(serviceProvider =>
               new TokenService(new ManagedIdentityApiAuthentication() { ApiBaseAddress = "https://at-qna-api.apprenticeships.education.gov.uk/", Identifier = "https://citizenazuresfabisgov.onmicrosoft.com/das-at-qna-as-ar" }, tokenLogger));

                services.AddHttpClient<IQnaApiClient, QnaApiClient>("QnaApiClient", config =>
                {
                    config.BaseAddress = new Uri(Configuration.QnaApiAuthentication.ApiBaseAddress);
                });

                config.For<IRoatpTokenService>().Use<TokenService>()
                    .Ctor<IClientApiAuthentication>().Is(Configuration.RoatpApiAuthentication)
                    .Ctor<string>().Is(_config["EnvironmentName"]);

                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>();
                config.For<IStandardsApiClient>().Use<StandardsApiClient>();
                config.For<IOppFinderApiClient>().Use<OppFinderApiClient>();
                config.For<IDashboardApiClient>().Use<DashboardApiClient>();
                config.For<IContactsApiClient>().Use<ContactsApiClient>();
                config.For<ISearchApiClient>().Use<SearchApiClient>();
                config.For<IEmailApiClient>().Use<EmailApiClient>();
                config.For<IValidationApiClient>().Use<ValidationApiClient>();
                config.For<ICertificateApiClient>().Use<CertificateApiClient>();
                config.For<ILoginApiClient>().Use<LoginApiClient>();
                config.For<IApplicationApiClient>().Use<ApplicationApiClient>();
                config.For<ILearnerDetailsApiClient>().Use<LearnerDetailApiClient>();
                config.For<IApprovalsLearnerApiClient>().Use<ApprovalsLearnerApiClient>();
                config.For<ILocationsApiClient>().Use<LocationsApiClient>();
                config.For<IStandardVersionClient>().Use<StandardVersionClient>();

                config.For<IQnaApiClient>().Use<QnaApiClient>();
                config.For<IRoatpApiClient>().Use<RoatpApiClient>();

                config.For<IAzureApiClient>().Use<AzureApiClient>().Ctor<string>().Is(Configuration.AzureApiAuthentication.ApiBaseAddress);

                config.For<IApiValidationService>().Use<ApiValidationService>();
                config.For<IDateTimeHelper>().Use<DateTimeHelper>();

                var mapper = services.AddMappings().CreateMapper();
                config.For<IMapper>().Use(mapper);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
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
