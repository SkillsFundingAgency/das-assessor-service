using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Enums;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.Orchestrators
{
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

            Organisation organisation;
            try
            {
                organisation = await _organisationsApiClient.Get(ukprn, ukprn);
            }
            catch (EntityNotFoundException)
            {
                return LoginResult.NotRegistered;
            }
                
            try
            {
                await GetContact(ukprn, username, email, displayName);
            }
            catch (EntityNotFoundException)
            {
                await CreateNewContact(ukprn, email, organisation, displayName, username);
            }
            return LoginResult.Valid;
        }

        private bool UserDoesNotHaveAcceptableRole(ClaimsPrincipal principal)
        {
            return !principal.HasClaim("http://schemas.portal.com/service", _config.Authentication.Role);
        }

        private async Task GetContact(string ukprn, string username, string email, string displayName)
        {
            var contact = await _contactsApiClient.GetByUsername(ukprn, username);

            await CheckStoredUserDetailsForUpdate(ukprn, email, displayName, contact);
        }

        private async Task CheckStoredUserDetailsForUpdate(string ukprn, string email, string displayName, Contact contact)
        {
            if (contact.ContactEmail != email || contact.ContactName != displayName)
            {
                await _contactsApiClient.Update(ukprn, new UpdateContactRequest()
                {
                    ContactEmail = email,
                    ContactName = displayName,
                    Id = contact.Id
                });
            }
        }

        private async Task CreateNewContact(string ukprn, string email, Organisation organisation, string displayName,
            string username)
        {
            var contact = await _contactsApiClient.Create(ukprn,
                new CreateContactRequest()
                {
                    ContactEmail = email,
                    OrganisationId = organisation.Id,
                    ContactName = displayName,
                    Username = username,
                    EndPointAssessorContactId = 1
                });

            await SetNewOrganisationPrimaryContact(ukprn, organisation, contact);
        }

        private async Task SetNewOrganisationPrimaryContact(string ukprn, Organisation organisation, Contact contact)
        {
            if (organisation.OrganisationStatus == OrganisationStatus.New)
            {
                await _organisationsApiClient.Update(ukprn, new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    PrimaryContactId = contact.Id
                });
            }
        }
    }
}