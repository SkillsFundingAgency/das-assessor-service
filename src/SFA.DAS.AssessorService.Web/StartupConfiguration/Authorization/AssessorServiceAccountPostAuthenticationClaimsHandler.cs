using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common.Exceptions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
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
            var claims = await GetClaims(tokenValidatedContext.Principal);

            if (claims.Any(claim => claim.Type == "ContactStatus" && string.Equals(claim.Value, ContactStatus.Deleted, StringComparison.OrdinalIgnoreCase)))
            {
                tokenValidatedContext.Response.Redirect("/Home/AccessDenied");
                tokenValidatedContext.HandleResponse();
            }

            return claims;
        }

        public async Task<IEnumerable<Claim>> GetClaims(ClaimsPrincipal principal)
        {
            var claims = new List<Claim>();
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var govLoginId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(govLoginId) && string.IsNullOrEmpty(email))
                return claims;

            var user = await ResolveContact(govLoginId, email);

            if (user?.Status == ContactStatus.Deleted)
            {
                claims.Add(new Claim("ContactStatus", user.Status));
                return claims;
            }

            if (user == null)
                return claims;

            await SyncEmailIfChanged(user, email);
            AddDisplayNameClaim(principal, user, claims);

            claims.Add(new Claim("UserId", user.Id.ToString()));
            claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", user.Username));
            claims.AddRange(await BuildOrganisationClaims(user));
            claims.Add(new Claim("display_name", user.DisplayName));
            claims.Add(new Claim("email", user.Email));

            await _contactsApiClient.UpdateFromGovLogin(new UpdateContactGovLoginRequest
            {
                GovIdentifier = govLoginId,
                ContactId = user.Id
            });

            await HandlePendingCallback(user, govLoginId);

            return claims;
        }

        private async Task<ContactResponse> ResolveContact(string govLoginId, string email)
        {
            ContactResponse user = null;

            try
            {
                user = await _contactsApiClient.GetContactByGovIdentifier(govLoginId);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("Failed to retrieve user by gov login.");
            }

            try
            {
                user ??= await _contactsApiClient.GetContactByEmail(email);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("Failed to retrieve user by email.");
            }

            return user;
        }

        private async Task SyncEmailIfChanged(ContactResponse user, string email)
        {
            if (string.Equals(user.Email, email, StringComparison.CurrentCultureIgnoreCase))
                return;

            await _contactsApiClient.UpdateEmail(new UpdateEmailRequest
            {
                NewEmail = email,
                GovUkIdentifier = user.GovUkIdentifier
            });
            user.Email = email;
        }

        private static void AddDisplayNameClaim(ClaimsPrincipal principal, ContactResponse user, List<Claim> claims)
        {
            var primaryIdentity = principal.Identities.FirstOrDefault();
            if (primaryIdentity == null || !string.IsNullOrEmpty(primaryIdentity.Name))
                return;

            primaryIdentity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
            if (!claims.Exists(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
            }
        }

        private async Task<IEnumerable<Claim>> BuildOrganisationClaims(ContactResponse user)
        {
            if (user.OrganisationId == null)
                return [];

            var organisation = await _organisationsApiClient.GetEpaOrganisationById(user.OrganisationId.ToString());
            if (organisation == null)
                return [];

            var orgName = organisation.OrganisationData?.LegalName
                ?? organisation.OrganisationData?.TradingName
                ?? organisation.Name;

            _sessionService.Set("OrganisationName", orgName);

            return
            [
                new Claim("http://schemas.portal.com/ukprn", organisation.Ukprn?.ToString() ?? ""),
                new Claim("http://schemas.portal.com/epaoid", organisation.OrganisationId)
            ];
        }

        private async Task HandlePendingCallback(ContactResponse user, string govLoginId)
        {
            if (user.Status != "Pending")
                return;

            await _contactsApiClient.Callback(new SignInCallback
            {
                SourceId = user.Id.ToString(),
                GovIdentifier = govLoginId
            });
        }
    }
}
