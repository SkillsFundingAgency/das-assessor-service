using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using StackExchange.Redis;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SFA.DAS.AssessorService.Application.Infrastructure;

namespace SFA.DAS.AssessorService.Web
{
    public class Startup
    { 
        private readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;
        private readonly IHostingEnvironment _env;
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public Startup(IConfiguration config, ILogger<Startup> logger, IHostingEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
        }

        private IWebConfiguration Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Configuration = ConfigurationService.GetConfig(_config["EnvironmentName"], _config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            //services.AddApplicationInsightsTelemetry();

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddAndConfigureAuthentication(Configuration, _logger, _env);
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });
            
            services.AddSingleton<IAuthorizationPolicyProvider, AssessorPolicyProvider>();
            
            services.AddSingleton<IAuthorizationHandler, ApplicationAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PrivilegeAuthorizationHandler>();

            services.AddMvc(options => { options.Filters.Add<CheckSessionFilter>();})
                .AddControllersAsServices()
                .AddSessionStateTempDataProvider()
                .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<Microsoft.AspNetCore.Mvc.ViewFeatures.IHtmlGenerator,CacheOverrideHtmlGenerator>();
            
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
            
            services.AddHttpClient<IRoatpApiClient, RoatpApiClient>("RoatpApiClient", cfg =>
                {
                    cfg.BaseAddress = new Uri(Configuration.RoatpApiAuthentication.ApiBaseAddress); //  "https://at-providers-api.apprenticeships.education.gov.uk"
                    cfg.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddHealthChecks();
            
            return ConfigureIoc(services);
        }        

        private IServiceProvider ConfigureIoc(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.WithDefaultConventions();
                });

                config.For<ITokenService>().Use<TokenService>();
                config.For<ITokenService>().Add<QnaTokenService>().Named("qnaTokenService");
                config.For<ITokenService>().Use<TokenService>().Ctor<bool>("useSandbox").Is(false); // Always false unless we want to start integrating with the sandbox environment;
                config.For<IAzureTokenService>().Use<AzureTokenService>();

                config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);
                config.For<IOppFinderSession>().Use<OppFinderSession>();

                config.For<IWebConfiguration>().Use(Configuration);
                
                config.For<IQnaApiClient>().Use<QnaApiClient>()
                  .Ctor<ITokenService>("qnaTokenService").Is(c => c.GetInstance<ITokenService>("qnaTokenService"))
                  .Ctor<string>().Is(Configuration.QnaApiAuthentication.ApiBaseAddress);
                
                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IStandardsApiClient>().Use<StandardsApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IOppFinderApiClient>().Use<OppFinderApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IDashboardApiClient>().Use<DashboardApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<ISearchApiClient>().Use<SearchApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IEmailApiClient>().Use<EmailApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IValidationApiClient>().Use<ValidationApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<ICertificateApiClient>().Use<CertificateApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<ILoginApiClient>().Use<LoginApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IApplicationApiClient>().Use<ApplicationApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<ILearnerDetailsApiClient>().Use<LearnerDetailApiClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);
                config.For<IAzureApiClient>().Use<AzureApiClient>().Ctor<string>().Is(Configuration.AzureApiAuthentication.ApiBaseAddress);
                config.For<IStandardVersionClient>().Use<StandardVersionClient>().Ctor<string>().Is(Configuration.AssessorApiAuthentication.ApiBaseAddress);

                config.For<IApiValidationService>().Use<ApiValidationService>();

                config.For<IDateTimeHelper>().Use<DateTimeHelper>();
                
               
                
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            MappingStartup.AddMappings();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseSecurityHeaders()
                .UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseRequestLocalization()
                .UseHealthChecks("/health")
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" }
                        //,constraints: new { controller = new NotEqualRouteContraint("find-an-assessment-opportunity") }
                        );
                });
        }        
    }
}
