using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

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
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".Assessors.Cookies";
                    if (!env.IsDevelopment())
                    {
                        options.Cookie.Domain = ".apprenticeships.education.gov.uk";
                    }
                    options.Cookie.HttpOnly = true;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                })
                .AddOpenIdConnect("oidc", options =>
                 {
                     options.CorrelationCookie = new CookieBuilder()
                     {
                         Name = ".Assessors.Correlation.",
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
                    options.Scope.Add("profile");

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

                             var contactApplyClient = context.HttpContext.RequestServices.GetRequiredService<IContactApplyClient>();
                             var organisationApplyClient = context.HttpContext.RequestServices.GetRequiredService<IOrganisationsApplyApiClient>();

                             var signInId = context.Principal.FindFirst("sub")?.Value;
                             var email = context.Principal.FindFirst("name")?.Value;
                             var familyName = context.Principal.FindFirst("family_name")?.Value;
                             var givenName = context.Principal.FindFirst("given_name")?.Value;

                             ContactResponse user = null;
                             try
                             {
                                 user = await contactClient.GetContactBySignInId(signInId);
                             }
                             catch (EntityNotFoundException)
                             {
                                 logger.LogInformation("Failed to retrieve user.");
                             }

                             if (user == null)
                             {
                                 //Do all this below if the user is not found in Assessor
                                 bool createNewContactWithNoOrg = false;
                                 logger.LogInformation("Trying to get user from apply to retrieve user.");

                                 var applyContact = await contactApplyClient.GetApplyContactBySignInId(Guid.Parse(signInId));
                                 if (applyContact != null)
                                 {
                                     //Check if organisation exist in assessor 
                                     ApplyTypes.Organisation applyOrganisation = null;
                                     try
                                     {
                                         applyOrganisation = await organisationApplyClient.GetOrganisationByUserId(applyContact.Id);
                                     }
                                     catch (EntityNotFoundException) {
                                         logger.LogInformation("Found contact in apply, but no organisation associated with it.");
                                         createNewContactWithNoOrg = true;
                                     }

                                     if (applyOrganisation != null)
                                     {
                                         //Start migrating apply contact into assessor
                                         OrganisationResponse assessorOrg = null;
                                         try
                                         {
                                             assessorOrg = await orgClient.GetOrganisationByName(applyOrganisation.Name);
                                         }
                                         catch (EntityNotFoundException)
                                         {
                                             logger.LogInformation("No organisation found in Assessor, hence no RoEPAO.");
                                             createNewContactWithNoOrg = true;
                                         }
                                         if (assessorOrg != null)
                                         {
                                             //Organisation exists in assessor so update contact with organisation
                                             var newContact = new Contact
                                             {
                                                 Id = applyContact.Id,
                                                 DisplayName = $"{applyContact.GivenNames} {applyContact.FamilyName}",
                                                 Email = applyContact.Email,
                                                 SignInId = Guid.Parse(signInId),
                                                 SignInType = applyContact.SigninType,
                                                 CreatedAt = applyContact.CreatedAt,
                                                 Username = applyContact.Email,
                                                 Title = "",
                                                 FamilyName = applyContact.FamilyName,
                                                 GivenNames = applyContact.GivenNames,
                                                 OrganisationId = assessorOrg.Id,
                                                 EndPointAssessorOrganisationId = assessorOrg.EndPointAssessorOrganisationId,
                                                 Status = "Live"
                                             };

                                             user = await CreateANewContact(newContact, contactClient, logger, signInId);

                                         }
                                     }
                                 }
                                 if (createNewContactWithNoOrg)
                                 {
                                     //Userexists in apply but associated with org not in assessor (ie not EPAO org) so create a new user
                                     logger.LogInformation("Creating new user.");
                                     var newContact = new Contact
                                     {
                                         Id = applyContact.Id,
                                         DisplayName = $"{givenName} {familyName}",
                                         Email = email,
                                         SignInId = Guid.Parse(signInId),
                                         SignInType = "ASLogin",
                                         CreatedAt = DateTime.UtcNow,
                                         Username = email,
                                         Title = "",
                                         FamilyName = familyName,
                                         GivenNames = givenName,
                                         OrganisationId = null,
                                         EndPointAssessorOrganisationId = null,
                                         Status = "Applying"
                                     };

                                     user = await CreateANewContact(newContact, contactClient, logger, signInId);
                                 }

                             }


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
                                         identity.AddClaim(new Claim("http://schemas.portal.com/service",
                                            Roles.ExternalApiAccess));
                                         identity.AddClaim(new Claim("http://schemas.portal.com/service",
                                            Roles.EpaoUser));
                                     }

                                     identity.AddClaim(new Claim("http://schemas.portal.com/ukprn",
                                        organisation?.Ukprn == null ? "" : organisation?.Ukprn.ToString()));

                                     var orgName = organisation.OrganisationData?.LegalName ??
                                                  organisation.OrganisationData?.TradingName;

                                     identity.AddClaim(new Claim("http://schemas.portal.com/orgname",
                                        orgName));

                                     identity.AddClaim(new Claim("http://schemas.portal.com/epaoid",
                                        organisation?.OrganisationId));
                                 }

                                 identity.AddClaim(new Claim("display_name", user?.DisplayName));
                                 identity.AddClaim(new Claim("email", user?.Email));

                                 //Todo: Need to determine privileges dynamically
                                 identity.AddClaim(new Claim("http://schemas.portal.com/service",
                                     Privileges.ManageUsers));
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

        private async static Task<ContactResponse> CreateANewContact(Contact newContact, IContactsApiClient contactClient, ILogger<Startup> logger, string signInId)
        {
            try
            {
                var contactResponse = await contactClient.CreateANewContactWithGivenId(newContact);
                await contactClient.AssociateDefaultRolesAndPrivileges(newContact);
                //try retrieveing contact again
                return await contactClient.GetContactBySignInId(signInId);
            }
            catch (Exception e)
            {
                logger.LogInformation($"CreateContactHandler Error: {e.Message} {e.StackTrace} {e.InnerException?.Message}");
                throw e;
            }
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