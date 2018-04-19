using System;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Authentication;
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
                    options.Events.OnSecurityTokenValidated = OnTokenValidated;
                    options.CallbackPath = "/Account/SignedIn";
                })
                .AddCookie(options =>
                {
                    options.ReturnUrlParameter = "/Account/SignedIn";
                    options.Cookie = new CookieBuilder() {Name = ".Assessors.Cookies"};
                });
        }


        private static Task OnTokenValidated(SecurityTokenValidatedContext context)
        {
            //var ukprn = (context.Principal.FindFirst("http://schemas.portal.com/ukprn"))?.Value;

            //var jwt = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm())
            //    .WithSecret(_configuration.Api.TokenEncodingKey)
            //    .Issuer("sfa.das.assessorservice")
            //    .Audience("sfa.das.assessorservice.api")
            //    .ExpirationTime(DateTime.Now.AddMinutes(5))
            //    .AddClaim("ukprn", ukprn)
            //    .Build();

            //context.HttpContext.Session.SetString(ukprn, jwt);

            return Task.FromResult(0);
        }
    }
}