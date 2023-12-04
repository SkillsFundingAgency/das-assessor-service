using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class AssessorServiceAccountPostAuthenticationClaimsHandler : ICustomClaims
    {
        private readonly ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler> _logger;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly ISessionService _sessionService;
        private readonly IWebConfiguration _webConfiguration;

        public AssessorServiceAccountPostAuthenticationClaimsHandler(
            ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler> logger,
            IContactsApiClient contactsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            ISessionService sessionService,
            IWebConfiguration webConfiguration)
        {
            _logger = logger;
            _contactsApiClient = contactsApiClient;
            _organisationsApiClient = organisationsApiClient;
            _sessionService = sessionService;
            _webConfiguration = webConfiguration;
        }
        
        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {

            var claims = new List<Claim>();                
            var signInId = tokenValidatedContext.Principal.FindFirst("sub")?.Value;
            var email = tokenValidatedContext.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var govLoginId = tokenValidatedContext.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ContactResponse user = null;
            if (!string.IsNullOrEmpty(signInId) || !string.IsNullOrEmpty(email))
            {
                try
                {
                    user = await _contactsApiClient.GetContactBySignInId(signInId);
                }
                catch (SFA.DAS.AssessorService.Application.Api.Client.Exceptions.EntityNotFoundException)
                {
                    _logger.LogInformation("Failed to retrieve user be Sign In Id.");
                }
                
                try
                {
                    user ??= await _contactsApiClient.GetContactByEmail(email);
                }
                catch (SFA.DAS.AssessorService.Application.Api.Client.Exceptions.EntityNotFoundException)
                {
                    _logger.LogInformation("Failed to retrieve user by email.");
                }
                
                try
                {
                    user ??= await _contactsApiClient.GetContactByGovIdentifier(govLoginId);
                }
                catch (SFA.DAS.AssessorService.Application.Api.Client.Exceptions.EntityNotFoundException)
                {
                    _logger.LogInformation("Failed to retrieve user by gov login.");
                }

                if (user?.Status == ContactStatus.Deleted)
                {
                    // Redirect to access denied page. 
                    tokenValidatedContext.Response.Redirect("/Home/AccessDenied");
                    tokenValidatedContext.HandleResponse();
                }

                if (_webConfiguration.UseGovSignIn 
                    && user != null 
                    && !string.Equals(user.Email, email, StringComparison.CurrentCultureIgnoreCase))
                {
                    // update the email address if the Gov Uk Sign In comes with different email address. 
                    await _contactsApiClient.UpdateEmail(new UpdateEmailRequest
                    {
                        NewEmail = email,
                        GovUkIdentifier = user.GovUkIdentifier
                    });
                    user.Email = email;
                }

                if (user != null)
                {
                    var primaryIdentity = tokenValidatedContext.Principal.Identities.FirstOrDefault();
                    if (primaryIdentity != null && string.IsNullOrEmpty(primaryIdentity.Name))
                    {
                        primaryIdentity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
                        if (!claims.Exists(c => c.Type == ClaimTypes.Name))
                        {
                            claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));    
                        }
                        
                    }

                    claims.Add(new Claim("UserId", user?.Id.ToString()));
                    claims.Add(new Claim(
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
                        user?.Username));
                    if (user.OrganisationId != null)
                    {
                        var organisation =
                            await _organisationsApiClient.GetEpaOrganisationById(user.OrganisationId?.ToString());

                        if (organisation != null)
                        {
                            claims.Add(new Claim("http://schemas.portal.com/ukprn",
                                organisation.Ukprn == null ? "" : organisation.Ukprn.ToString()));

                            var orgName = organisation.OrganisationData?.LegalName ??
                                          organisation.OrganisationData?.TradingName ??
                                          organisation.Name;
                        
                            _sessionService.Set("OrganisationName", orgName);

                            claims.Add(new Claim("http://schemas.portal.com/epaoid", organisation.OrganisationId));    
                        }
                    }

                    claims.Add(new Claim("display_name", user?.DisplayName));
                    claims.Add(new Claim("email", user?.Email));
                    var userSignInId = user.SignInId ?? (string.IsNullOrEmpty(signInId) ? Guid.NewGuid() : Guid.Parse(signInId));
                    var response = await _contactsApiClient.UpdateFromGovLogin(new UpdateContactGovLoginRequest
                    {
                        GovIdentifier = govLoginId,
                        SignInId = userSignInId,
                        ContactId = user.Id
                    });
                    if (user.SignInId == null && !string.IsNullOrEmpty(govLoginId))
                    {
                        claims.Add(new Claim("sub", response.SignInId.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim("sub", user.SignInId.ToString()));
                    }
                }
            }

            return claims;
        }
    }    
}

