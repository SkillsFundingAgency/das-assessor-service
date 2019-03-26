using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(options => { 
                    options.Cookie.Name = ".Assessor.Cookies";
                    options.Cookie.HttpOnly = true;
                })
                .AddOpenIdConnect("oidc",options =>
                {
                    options.CorrelationCookie = new CookieBuilder()
                    {
                        Name = ".Assessor.Correlation.", 
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    
                    options.SignInScheme = "Cookies";
                    options.Authority = _configuration.DfeSignIn.MetadataAddress;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = _configuration.DfeSignIn.ClientId;

                    options.Scope.Clear();
                    options.Scope.Add("openid");

                    options.SaveTokens = true;

                    options.DisableTelemetry = true;
                    options.Events = new OpenIdConnectEvents
                    {
                        // Sometimes, problems in the OIDC provider (such as session timeouts)
                        // Redirect the user to the /auth/cb endpoint. ASP.NET Core middleware interprets this by default
                        // as a successful authentication and throws in surprise when it doesn't find an authorization code.
                        // This override ensures that these cases redirect to the root.
                        OnMessageReceived = context =>
                        {
                            var isSpuriousAuthCbRequest =
                                context.Request.Path == options.CallbackPath &&
                                context.Request.Method == "GET" &&
                                !context.Request.Query.ContainsKey("code");

                            if (isSpuriousAuthCbRequest)
                            {
                                context.HandleResponse();
                                context.Response.StatusCode = 302;
                                context.Response.Headers["Location"] = "/";
                            }

                            return Task.CompletedTask;
                        },

                        // Sometimes the auth flow fails. The most commonly observed causes for this are
                        // Cookie correlation failures, caused by obscure load balancing stuff.
                        // In these cases, rather than send user to a 500 page, prompt them to re-authenticate.
                        // This is derived from the recommended approach: https://github.com/aspnet/Security/issues/1165
                        OnRemoteFailure = ctx =>
                        {
                            ctx.Response.Redirect( "/");
                            ctx.HandleResponse();
                            return Task.FromResult(0);
                        },

                        OnTokenValidated = async context =>
                        {
                            var identity = new ClaimsIdentity();
                            var contactClient = context.HttpContext.RequestServices.GetRequiredService<IContactsApiClient>();
                            var orgClient = context.HttpContext.RequestServices
                                .GetRequiredService<IOrganisationsApiClient>();
                            var signInId = context.Principal.FindFirst("sub")?.Value;
                            var user = await contactClient.GetContactBySignInId(signInId);
                            if (user != null)
                            {
                                identity.AddClaim(new Claim("UserId", user?.Id.ToString()));
                                identity.AddClaim(new Claim(
                                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
                                    user?.Username));
                                if (user.EndPointAssessorOrganisationId != null)
                                {
                                    var organisation =
                                        await orgClient.GetEpaOrganisation(user.EndPointAssessorOrganisationId);

                                    if (organisation.ApiEnabled && !string.IsNullOrEmpty(organisation.ApiUser))
                                    {
                                        identity.AddClaim(new Claim("http://schemas.portal.com/service", Roles.ExternalApiAccess));
                                        identity.AddClaim(new Claim("http://schemas.portal.com/service", Roles.EpaoUser));
                                    }
                                    identity.AddClaim(new Claim("http://schemas.portal.com/ukprn",
                                        organisation?.Ukprn.ToString()));
                                }
                                identity.AddClaim(new Claim("display_name", user?.DisplayName));
                                identity.AddClaim(new Claim("email", user?.Email));

                                //Todo: Need to determine privileges dynamically
                                identity.AddClaim(new Claim("http://schemas.portal.com/service", Privileges.ManageUsers));
                            }

                            context.Principal.AddIdentity(identity);
                        }

                    };
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
                options.AddPolicy(Policies.SuperUserPolicy,
                    policy =>
                    {
                        policy.RequireAssertion(context =>
                            context.User.HasClaim("http://schemas.portal.com/service", Privileges.ManageUsers)
                        );
                    });
            });
        }
    }

    public class Policies
    {
        public const string ExternalApiAccess = "ExternalApiAccess";
        public const string SuperUserPolicy = "SuperUserPolicy";
    }
    
    public class Roles
    {
        public const string ExternalApiAccess = "EPI";
        public const string EpaoUser = "EPA";
    }

    public class Privileges
    {
        public const string ManageUsers = "ManageUser";
    }
}