using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
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
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Services;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using StructureMap;
using CheckSessionFilter = SFA.DAS.AssessorService.Web.Staff.Infrastructure.CheckSessionFilter;
using ISessionService = SFA.DAS.AssessorService.Web.Staff.Infrastructure.ISessionService;

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
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
            AddAuthentication(services);
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });
            services.AddMvc(options => { options.Filters.Add<CheckSessionFilter>(); })
                 .AddMvcOptions(m => m.ModelMetadataDetailsProviders.Add(new HumanizerMetadataProvider()))
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                 .SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });
            
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = ApplicationConfiguration.SessionRedisConnectionString;
            });          
            MappingStartup.AddMappings();
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
                config.For<CertificateDateViewModelValidator>().Use<CertificateDateViewModelValidator>();
                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(ApplicationConfiguration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(ApplicationConfiguration.AssessmentOrgsApiClientBaseUrl));
                config.For<IIfaStandardsApiClient>().Use(() => new IfaStandardsApiClient(ApplicationConfiguration.IfaApiClientBaseUrl));
                config.For<IAzureTokenService>().Use<AzureTokenService>();
                config.For<IAzureApiClient>().Use<AzureApiClient>().Ctor<string>("baseUri").Is(ApplicationConfiguration.AzureApiAuthentication.ApiBaseAddress)
                                                                   .Ctor<string>("productId").Is(ApplicationConfiguration.AzureApiAuthentication.ProductId)
                                                                   .Ctor<string>("groupId").Is(ApplicationConfiguration.AzureApiAuthentication.GroupId)
                                                                   .Ctor<string>("requestBaseUri").Is(ApplicationConfiguration.AzureApiAuthentication.RequestBaseAddress);
                config.For<CacheService>().Use<CacheService>();
                config.For<CertificateLearnerStartDateViewModelValidator>()
                    .Use<CertificateLearnerStartDateViewModelValidator>();
                config.For<IRegisterValidator>().Use<RegisterValidator>();

                config.For<IStandardService>().Use<StandardService>();
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
                options.TokenValidationParameters.RoleClaimType = Domain.Roles.RoleClaimType;
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