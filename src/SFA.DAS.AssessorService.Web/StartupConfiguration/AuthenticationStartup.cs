using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class AuthenticationStartup
    {
        private static IWebConfiguration _configuration;

        public static void AddAndConfigureAuthentication(this IServiceCollection services, IWebConfiguration configuration)
        {
            _configuration = configuration;

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = configuration.Authentication.WtRealm;
                    options.MetadataAddress = configuration.Authentication.MetadataAddress;
                    options.CallbackPath = "/Account/SignedIn";
                    options.CorrelationCookie = new CookieBuilder() { Name = ".Assessors.Correlation"};
                })
                .AddCookie(options =>
                {
                    options.ReturnUrlParameter = "/Account/SignedIn";
                    options.Cookie = new CookieBuilder() {Name = ".Assessors.Cookies"};
                });
        }
    }
}