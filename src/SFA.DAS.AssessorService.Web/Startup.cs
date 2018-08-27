using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using StructureMap;
using SessionCache = SFA.DAS.AssessorService.Application.Api.Client.SessionCache;

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

        public IWebConfiguration Configuration { get; set; }

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
            services.AddMvc(options => { options.Filters.Add<CheckSessionFilter>(); })
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

            AddHttpClientService<ICertificateApiClient, CertificateApiClient>(services);
            AddHttpClientService<ILoginApiClient, LoginApiClient>(services);
            AddHttpClientService<ISearchApiClient, SearchApiClient>(services);
            AddHttpClientService<IContactsApiClient, ContactsApiClient>(services);
            AddHttpClientService<IOrganisationsApiClient, OrganisationsApiClient>(services);

            return ConfigureIOC(services);
        }

        public void AddHttpClientService<T, U>(IServiceCollection services)
            where T: class
            where U: ApiClientBase, T
        {
            services.AddHttpClient<T, U> (config =>
                {
                    config.BaseAddress = new Uri(Configuration.ClientApiAuthentication.ApiBaseAddress);
                    config.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
        }

        private IServiceProvider ConfigureIOC(IServiceCollection services)
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
                //config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                //config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                //config.For<ISearchApiClient>().Use<SearchApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                //config.For<IApiClient>().Use<ApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                //config.For<ICertificateApiClient>().Use<CertificateApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                //config.For<ILoginApiClient>().Use<LoginApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);

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

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
