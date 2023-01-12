using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public static class AuthenticationStartup
    {
        private static IWebConfiguration _configuration;

        public static void AddAndConfigureAuthentication(this IServiceCollection services,
            IWebConfiguration configuration, ILogger<Startup> logger, IHostingEnvironment env)
        {
            _configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = ".Assessors.Cookies";
                    options.Cookie.HttpOnly = true;

                    if (!env.IsDevelopment())
                    {
                        options.Cookie.Domain = ".apprenticeships.education.gov.uk";
                    }

                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.CorrelationCookie = new CookieBuilder()
                    {
                        Name = ".Assessors.Correlation.",
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };

                    options.SignInScheme = "Cookies";
                    options.Authority = _configuration.LoginService.MetadataAddress;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = _configuration.LoginService.ClientId;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    options.SaveTokens = true;

                    options.DisableTelemetry = true;
                    options.Events = new OpenIdConnectEvents
                    {
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

                        OnRemoteFailure = ctx =>
                        {
                            ctx.Response.Redirect("/");
                            ctx.HandleResponse();
                            return Task.FromResult(0);
                        },

                        OnTokenValidated = async context =>
                        {
                            var identity = new ClaimsIdentity();
                            var contactClient = context.HttpContext.RequestServices.GetRequiredService<IContactsApiClient>();
                            var orgClient = context.HttpContext.RequestServices
                                .GetRequiredService<IOrganisationsApiClient>();
                            var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();


                            var signInId = context.Principal.FindFirst("sub")?.Value;

                            ContactResponse user = null;
                            if (!string.IsNullOrEmpty(signInId))
                            {
                                try
                                {
                                    user = await contactClient.GetContactBySignInId(signInId);
                                }
                                catch (EntityNotFoundException)
                                {
                                    logger.LogInformation("Failed to retrieve user.");
                                }

                                if (user?.Status == ContactStatus.Deleted)
                                {
                                    // Redirect to access denied page. 
                                    context.Response.Redirect("/Home/AccessDenied");
                                    context.HandleResponse();
                                }

                                if (user != null)
                                {
                                    var primaryIdentity = context.Principal.Identities.FirstOrDefault();
                                    if (primaryIdentity != null && string.IsNullOrEmpty(primaryIdentity.Name))
                                    {
                                        primaryIdentity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
                                    }

                                    identity.AddClaim(new Claim("UserId", user?.Id.ToString()));
                                    identity.AddClaim(new Claim(
                                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
                                        user?.Username));
                                    if (user.OrganisationId != null)
                                    {
                                        var organisation =
                                            await orgClient.GetEpaOrganisationById(user.OrganisationId?.ToString());

                                        identity.AddClaim(new Claim("http://schemas.portal.com/ukprn",
                                            organisation?.Ukprn == null ? "" : organisation?.Ukprn.ToString()));

                                        var orgName = organisation.OrganisationData?.LegalName ??
                                                      organisation.OrganisationData?.TradingName ??
                                                      organisation.Name;

                                        sessionService.Set("OrganisationName", orgName);

                                        identity.AddClaim(new Claim("http://schemas.portal.com/epaoid",
                                            organisation?.OrganisationId));
                                    }

                                    identity.AddClaim(new Claim("display_name", user?.DisplayName));
                                    identity.AddClaim(new Claim("email", user?.Email));
                                }
                            }

                            context.Principal.AddIdentity(identity);
                        }
                    };
                });


            services.AddAuthorization();
        }
    }
}