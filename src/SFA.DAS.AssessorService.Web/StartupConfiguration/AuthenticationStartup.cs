using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class AuthenticationStartup
    {
        private static IWebConfiguration _configuration;

        public static void AddAndConfigureAuthentication(this IServiceCollection services,
            IWebConfiguration configuration, ILogger<Startup> logger)
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
                    options.Events = new WsFederationEvents()
                    {
                        OnMessageReceived = context =>
                        {
                           logger.LogInformation($"WSFED MessageReceived: {context.ProtocolMessage.Wresult}");
                            return Task.FromResult(0);
                        },
                        OnRedirectToIdentityProvider = context =>
                        {
                            logger.LogInformation($"WSFED RedirectToIdentityProvider: {context.ProtocolMessage.ToString()}");

                            return Task.FromResult(0);
                        },
                        OnSecurityTokenReceived = context =>
                        {
                            logger.LogInformation($"WSFED SecurityTokenReceived: {context.ProtocolMessage.ToString()}");
                            return Task.FromResult(0);
                        },
                        OnSecurityTokenValidated = context =>
                        {
                            logger.LogInformation($"WSFED SecurityTokenValidated: {context.ProtocolMessage.ToString()}");
                            return Task.FromResult(0);
                        },
                        OnAuthenticationFailed = context =>
                        {
                            logger.LogInformation($"WSFED AuthenticationFailed: {context.ProtocolMessage.ToString()}");
                            return Task.FromResult(0);
                        },
                        OnRemoteSignOut = context =>
                        {
                            logger.LogInformation($"WSFED RemoteSignOut: {context.ProtocolMessage.ToString()}");
                            return Task.FromResult(0);
                        }, 
                        OnRemoteFailure = context =>
                        {
                            logger.LogInformation($"WSFED RemoteFailure: {context.Failure.Message}");
                            return Task.FromResult(0);
                        }
                    };
                })
                .AddCookie(options =>
                {
                    options.Cookie = new CookieBuilder() {Name = ".Assessors.Cookies", HttpOnly = true};
                });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ExternalApiAccess,
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                            context.User.HasClaim("http://schemas.portal.com/service", Roles.ExternalApiAccess)
                            && context.User.HasClaim("http://schemas.portal.com/service", Roles.EpaoUser)
                            );
                    });
            });
        }
    }

    public class Policies
    {
        public const string ExternalApiAccess = "ExternalApiAccess";
    }
    
    public class Roles
    {
        public const string ExternalApiAccess = "EPI";
        public const string EpaoUser = "EPA";
    }
}