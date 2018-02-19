using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Services;
using StructureMap;
using SessionCache = SFA.DAS.AssessorService.Application.Api.Client.SessionCache;

namespace SFA.DAS.AssessorService.Web
{
    public class Startup
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
            Configuration = ConfigurationService.GetConfig(_config["Environment"], _config["ConnectionStrings:Storage"], Version, ServiceName).Result;
        }

        public IWebConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = Configuration.Authentication.WtRealm;
                    options.MetadataAddress = Configuration.Authentication.MetadataAddress;
                    options.Events.OnSecurityTokenValidated = OnTokenValidated;
                    options.CallbackPath = "/Account/SignedIn";
                })
                .AddCookie(options => { options.ReturnUrlParameter = "/Account/SignedIn"; });

            services.AddMvc().AddControllersAsServices().AddSessionStateTempDataProvider();

            services.AddSession();

            return ConfigureIOC(services);
        }

        private Task OnTokenValidated(SecurityTokenValidatedContext context)
        {
            var ukprn = (context.Principal.FindFirst("http://schemas.portal.com/ukprn"))?.Value;

            var claims = new[]
            {
                new Claim("ukprn", ukprn, ClaimValueTypes.String)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Api.TokenEncodingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "sfa.das.assessorservice",
                audience: "sfa.das.assessorservice.api",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            context.HttpContext.Session.SetString(ukprn, jwt);

            

            return Task.FromResult(0);
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

                config.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Configuration.Api.ApiBaseAddress);

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