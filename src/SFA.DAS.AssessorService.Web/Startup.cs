using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public Startup(IConfiguration config)
        {
            Configuration = ConfigurationService.GetConfig(config["Environment"], config["ConnectionStrings:Storage"], Version, ServiceName).Result;
        }

        public IWebConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddAndConfigureAuthentication(Configuration);
            services.AddMvc().AddControllersAsServices().AddSessionStateTempDataProvider().AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());
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
                config.For<ICache>().Use<SessionCache>();
                config.For<ITokenService>().Use<TokenService>();
                config.For<IWebConfiguration>().Use(Configuration);
                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<IContactsApiClient>().Use<ContactsApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);
                config.For<ISearchApiClient>().Use<SearchApiClient>().Ctor<string>().Is(Configuration.ClientApiAuthentication.ApiBaseAddress);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

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
            
            app.UseStaticFiles()
                .UseSession()
                .UseAuthentication()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}