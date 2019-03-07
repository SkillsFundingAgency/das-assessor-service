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

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options => { 
                    options.Cookie.Name = ".Apply.Cookies";
                    options.Cookie.HttpOnly = true;
                })
                .AddOpenIdConnect(options =>
                {
                    options.CorrelationCookie = new CookieBuilder()
                    {
                        Name = ".Apply.Correlation.", 
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.MetadataAddress = "https://signin-test-oidc-as.azurewebsites.net/.well-known/openid-configuration";

                    options.ClientId = "DasAssessorServivce";
                    const string envKeyClientSecret = "tussock-sentient-onshore";
                    var clientSecret = "tussock-sentient-onshore";
                    if (string.IsNullOrWhiteSpace(clientSecret))
                    {
                        throw new Exception("Missing environment variable " + envKeyClientSecret +
                                            " - get this from the DfE Sign-in team.");
                    }

                    options.ClientSecret = clientSecret;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // using this property would align the expiration of the cookie
                    // with the expiration of the identity token
                    // UseTokenLifetime = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("email");
                    options.Scope.Add("profile");

                    options.Scope.Add("offline_access");

                    options.SaveTokens = true;
                    //options.CallbackPath = new PathString(Configuration["auth:oidc:callbackPath"]);
                    //options.SignedOutCallbackPath = new PathString("/SignedOut");
                    //options.SignedOutRedirectUri = new PathString("/SignedOut");
                    options.SecurityTokenValidator = new JwtSecurityTokenHandler
                    {
                        InboundClaimTypeMap = new Dictionary<string, string>(),
                        TokenLifetimeInMinutes = 20,
                        SetDefaultTimesOnTokenCreation = true,
                    };
                    options.ProtocolValidator = new OpenIdConnectProtocolValidator
                    {
                        RequireSub = true,
                        RequireStateValidation = false,
                        NonceLifetime = TimeSpan.FromMinutes(15)
                    };

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
                            ctx.Response.Redirect(ctx.Failure.Message.Contains("Could not find contact")
                                ? configuration.ApplyBaseAddress
                                : "/");
                            ctx.HandleResponse();
                            return Task.FromResult(0);
                        },

                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.Prompt = "consent";
                            return Task.CompletedTask;
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
                                    identity.AddClaim(new Claim("http://schemas.portal.com/ukprn",
                                        organisation?.Ukprn.ToString()));
                                }
                            }

                            context.Principal.AddIdentity(identity);
                        }

                    };
                });


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(Policies.ExternalApiAccess,
            //        policy =>
            //        {
            //            policy.RequireAssertion(context =>
            //                context.User.HasClaim("http://schemas.portal.com/service", Roles.ExternalApiAccess)
            //                && context.User.HasClaim("http://schemas.portal.com/service", Roles.EpaoUser)
            //                );
            //        });
            //});
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