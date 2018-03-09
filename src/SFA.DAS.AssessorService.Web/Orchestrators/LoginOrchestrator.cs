using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.Orchestrators
{
    using Domain.Consts;

    public class LoginOrchestrator : ILoginOrchestrator
    {
        private readonly IWebConfiguration _config;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IContactsApiClient _contactsApiClient;

        public LoginOrchestrator(IWebConfiguration config, IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient)
        {
            _config = config;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
        }

        public async Task<LoginResult> Login(ClaimsPrincipal principal)
        {
            var ukprn = principal.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var email = principal.FindFirst("http://schemas.portal.com/mail")?.Value;
            var displayName = principal.FindFirst("http://schemas.portal.com/displayname")?.Value;

            if (UserDoesNotHaveAcceptableRole(principal))
            {
                return LoginResult.InvalidRole;
            }

            OrganisationResponse organisation;
            try
            {
                organisation = await _organisationsApiClient.Get(ukprn);
            }
            catch (EntityNotFoundException)
            {
                return LoginResult.NotRegistered;
            }
                
            try
            {
                await GetContact(username, email, displayName);
            }
            catch (EntityNotFoundException)
            {
                await CreateNewContact(email, organisation, displayName, username);
            }
            return LoginResult.Valid;
        }

        private bool UserDoesNotHaveAcceptableRole(ClaimsPrincipal principal)
        {
            return !principal.HasClaim("http://schemas.portal.com/service", _config.Authentication.Role);
        }

        private async Task GetContact(string username, string email, string displayName)
        {
            var contact = await _contactsApiClient.GetByUsername(username);

            await CheckStoredUserDetailsForUpdate(contact.Username, email, displayName, contact);
        }

        private async Task CheckStoredUserDetailsForUpdate(string userName, string email, string displayName, ContactResponse contactResponse)
        {
            if (contactResponse.Email != email || contactResponse.DisplayName != displayName)
            {
                await _contactsApiClient.Update(new UpdateContactRequest()
                {
                    Email = email,
                    DisplayName = displayName,
                    UserName = userName
                });
            }
        }

        private async Task CreateNewContact(string email, OrganisationResponse organisation, string displayName,
            string username)
        {
            var contact = await _contactsApiClient.Create(
                new CreateContactRequest()
                {
                    Email = email,
                    DisplayName = displayName,
                    UserName = username,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
                });

            await SetNewOrganisationPrimaryContact(organisation, contact);
        }

        private async Task SetNewOrganisationPrimaryContact(OrganisationResponse organisation, ContactResponse contactResponse)
        {
            if (organisation.Status == OrganisationStatus.New)
            {
                await _organisationsApiClient.Update(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    PrimaryContact = contactResponse.Username
                });
            }
        }
    }
}