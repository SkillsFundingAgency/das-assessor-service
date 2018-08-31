using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using StructureMap;

namespace SFA.DAS.AssessorService.Web.Staff
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";
        public IConfiguration Configuration { get; }
        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // Default is true, make it false
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            services.AddHttpClient<ApiClient>("ApiClient", config =>
            {
                config.BaseAddress = new Uri(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                config.DefaultRequestHeaders.Add("Accept", "Application/json");
            });

            AddAuthentication(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            //if (_env.IsDevelopment())
            //{
            //    services.AddDistributedMemoryCache();
            //}
            //else
            //{
            //    try
            //    {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = ApplicationConfiguration.SessionRedisConnectionString;
            });
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e, $"Error setting redis for session.  Conn: {ApplicationConfiguration.SessionRedisConnectionString}");
            //        throw;
            //    }
            //}

            return ConfigureIoC(services);

        }

        private IServiceProvider ConfigureIoC(IServiceCollection services)
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
                config.For<IWebConfiguration>().Use(ApplicationConfiguration);
                config.For<ISessionService>().Use<SessionService>().Ctor<string>().Is(_env.EnvironmentName);

                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IAzureTokenService>().Use<AzureTokenService>();
                config.For<IAzureApiClient>().Use<AzureApiClient>().Ctor<string>().Is(ApplicationConfiguration.AzureApiAuthentication.ApiBaseAddress);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
            }).AddWsFederation(options =>
            {
                options.Wtrealm = ApplicationConfiguration.StaffAuthentication.WtRealm;
                options.MetadataAddress = ApplicationConfiguration.StaffAuthentication.MetadataAddress;
            }).AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
