using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Services;
using SFA.DAS.AssessorService.Web.Settings;
using StructureMap;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options =>
                {
                    Configuration.Bind("AuthOptions", options);
                    AuthOptions.Settings = options;
                })
            .AddCookie();

            services.AddMvc().AddControllersAsServices().AddSessionStateTempDataProvider();
         
            services.AddSession();

            return ConfigureIOC(services);
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

                config.For<IHttpClient>().Use<StandardHttpClient>();
                config.For<ICache>().Use<Services.SessionCache>();

                //Populate the container using the service collection
                config.Populate(services);
            });
            
           // WebConfiguration configuration = GetConfiguration();

            return container.GetInstance<IServiceProvider>();
        }

        private const string ServiceName = "SFA.DAS.Support.Portal";
        private const string Version = "1.0";

        //private WebConfiguration GetConfiguration()
        //{
        //    //var environment = CloudConfigurationManager.GetSetting("EnvironmentName");

        //    //var storageConnectionString = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString");

        //    //if (environment == null) throw new ArgumentNullException(nameof(environment));
        //    //if (storageConnectionString == null) throw new ArgumentNullException(nameof(storageConnectionString));


        //    //var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString); ;

        //    //var configurationOptions = new ConfigurationOptions(ServiceName, environment, Version);

        //    //var configurationService = new ConfigurationService(configurationRepository, configurationOptions);

        //    //var webConfiguration = configurationService.Get<WebConfiguration>();

        //    //if (webConfiguration == null) throw new ArgumentOutOfRangeException($"The requried configuration settings were not retrieved, please check the environmentName case, and the configuration connection string is correct.");

        //    //return webConfiguration;
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
