using System;
using System.Collections.Generic;
using System.Globalization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using StructureMap;

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
            services.AddAndConfigureAuthentication(Configuration, _logger);
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });
            services.AddMvc(options => { options.Filters.Add<CheckSessionFilter>();})
                .AddControllersAsServices()
                .AddSessionStateTempDataProvider()
                .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".Assessors.AntiForgery", HttpOnly = true });

            if (_env.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
                
            }
            else
            {
                try
                {
                    services.AddDistributedRedisCache(options =>
                    {
                        options.Configuration = Configuration.SessionRedisConnectionString;
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error setting redis for session.  Conn: {Configuration.SessionRedisConnectionString}");
                    throw;
                }
            }

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            
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

                //config.For<ICache>().Use<SessionCache>();
                config.For<ITokenService>().Use<TokenService>();
                config.For<IWebConfiguration>().Use(Configuration);
                config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);
                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IStandardsApiClient>().Use<StandardsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<ISearchApiClient>().Use<SearchApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<ICertificateApiClient>().Use<CertificateApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(Configuration.AssessmentOrgsApiClientBaseUrl));
                config.For<IIfaStandardsApiClient>().Use(() => new IfaStandardsApiClient(Configuration.AssessmentOrgsApiClientBaseUrl));
                config.For<ILoginApiClient>().Use<LoginApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);

                config.For<IAzureTokenService>().Use<AzureTokenService>();
                config.For<IAzureApiClient>().Use<AzureApiClient>().Ctor<string>().Is(Configuration.AzureApiAuthentication.ApiBaseAddress);

                config.For<IStandardServiceClient>().Use<StandardServiceClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);

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
            app.UseStaticFiles()
                .UseSession(new SessionOptions() { Cookie = new CookieBuilder() { Name = ".Assessors.Session", HttpOnly = true } })
                .UseAuthentication()
                .UseRequestLocalization()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
        }        
    }
}
