﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
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
                    //options.SignedOutCallbackPath = new PathString(Configuration["auth:oidc:signedOutCallbackPath"]);
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
                            ctx.Response.Redirect("/");
                            ctx.HandleResponse();
                            return Task.FromResult(0);
                        },

                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.Prompt = "consent";
                            return Task.CompletedTask;
                        },

                        // that event is called after the OIDC middleware received the auhorisation code,
                        // redeemed it for an access token and a refresh token,
                        // and validated the identity token
//                        OnTokenValidated = x =>
//                        {
//                            // store both access and refresh token in the claims - hence in the cookie
//                            var identity = (ClaimsIdentity) x.Principal.Identity;
//                            identity.AddClaims(new[]
//                            {
//                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
//                                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
//                            });
//
//                            // so that we don't issue a session cookie but one with a fixed expiration
//                            x.Properties.IsPersistent = true;
//
//                            return Task.CompletedTask;
//                        }
                    };
                });
            
//            services.AddAuthentication(sharedOptions =>
//                {
//                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
//                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
//                })
//                .AddWsFederation(options =>
//                {
//                    options.Wtrealm = configuration.Authentication.WtRealm;
//                    options.MetadataAddress = configuration.Authentication.MetadataAddress;
//                    options.Events = new WsFederationEvents()
//                    {
//                        OnMessageReceived = context =>
//                        {
//                           logger.LogInformation($"WSFED MessageReceived: {context.ProtocolMessage.Wresult}");
//                            return Task.FromResult(0);
//                        },
//                        OnRedirectToIdentityProvider = context =>
//                        {
//                            logger.LogInformation($"WSFED RedirectToIdentityProvider: {context.ProtocolMessage.ToString()}");
//
//                            return Task.FromResult(0);
//                        },
//                        OnSecurityTokenReceived = context =>
//                        {
//                            logger.LogInformation($"WSFED SecurityTokenReceived: {context.ProtocolMessage.ToString()}");
//                            return Task.FromResult(0);
//                        },
//                        OnSecurityTokenValidated = context =>
//                        {
//                            logger.LogInformation($"WSFED SecurityTokenValidated: {context.ProtocolMessage.ToString()}");
//                            return Task.FromResult(0);
//                        },
//                        OnAuthenticationFailed = context =>
//                        {
//                            logger.LogInformation($"WSFED AuthenticationFailed: {context.ProtocolMessage.ToString()}");
//                            return Task.FromResult(0);
//                        },
//                        OnRemoteSignOut = context =>
//                        {
//                            logger.LogInformation($"WSFED RemoteSignOut: {context.ProtocolMessage.ToString()}");
//                            return Task.FromResult(0);
//                        }, 
//                        OnRemoteFailure = context =>
//                        {
//                            logger.LogInformation($"WSFED RemoteFailure: {context.Failure.Message}");
//                            return Task.FromResult(0);
//                        }
//                    };
//                })
//                .AddCookie(options =>
//                {
//                    options.Cookie = new CookieBuilder() {Name = ".Assessors.Cookies", HttpOnly = true};
//                });
            
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