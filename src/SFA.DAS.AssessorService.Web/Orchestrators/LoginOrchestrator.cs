using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginOrchestrator> _logger;

        public LoginOrchestrator(IWebConfiguration config, IOrganisationsApiClient organisationsApiClient, 
            IContactsApiClient contactsApiClient, ILogger<LoginOrchestrator> logger)
        {
            _config = config;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
            _logger = logger;
        }

        public async Task<LoginResult> Login(HttpContext context)
        {
            _logger.LogInformation("Start of Login");

            var ukprn = context.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var username = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var email = context.User.FindFirst("http://schemas.portal.com/mail")?.Value;
            var displayName = context.User.FindFirst("http://schemas.portal.com/displayname")?.Value;

            _logger.LogInformation($"Claims: ukprn: {ukprn}, username: {username}, email : {email}, displayName: {displayName}");

            if (UserDoesNotHaveAcceptableRole(context.User))
            {
                _logger.LogInformation("Invalid Role");
                return LoginResult.InvalidRole;
            }
            
            _logger.LogInformation("Role is good");

            OrganisationResponse organisation;
            try
            {
                _logger.LogInformation($"Getting Org with ukprn: {ukprn}");
                organisation = await _organisationsApiClient.Get(ukprn);
                _logger.LogInformation($"Got Org with ukprn: {ukprn}, Id: {organisation.EndPointAssessorOrganisationId}");
                context.Session.SetString("OrganisationName", organisation.EndPointAssessorName);
                _logger.LogInformation($"Set Org name in Session");
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation($"Org not registered");
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
            _logger.LogInformation($"Getting Contact with username: {username}");
            var contact = await _contactsApiClient.GetByUsername(username);
            _logger.LogInformation($"Got Existing Contact");
            await CheckStoredUserDetailsForUpdate(contact.UserName, email, displayName, contact);
        }

        private async Task CheckStoredUserDetailsForUpdate(string userName, string email, string displayName, ContactResponse contactResponse)
        {
            if (contactResponse.Email != email || contactResponse.DisplayName != displayName)
            {
                _logger.LogInformation($"Existing contact has updated details.  Updating");
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
            _logger.LogInformation($"Creating new contact.  Email: {email}, Username: {username}");
            var contact = await _contactsApiClient.Create(
                new CreateContactRequest()
                {
                    Email = email,
                    DisplayName = displayName,
                    UserName = username,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
                });

            _logger.LogInformation($"New contact created");

            await SetNewOrganisationPrimaryContact(organisation, contact);
        }

        private async Task SetNewOrganisationPrimaryContact(OrganisationResponse organisation, ContactResponse contactResponse)
        {
            if (organisation.Status == OrganisationStatus.New)
            {
                _logger.LogInformation($"Org status is New. Setting Org {organisation.EndPointAssessorUkprn} with primary contact of {contactResponse.UserName}");
                await _organisationsApiClient.Update(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    PrimaryContact = contactResponse.UserName
                });
            }
        }
    }
}