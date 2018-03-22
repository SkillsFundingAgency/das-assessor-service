using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly ILogger<LoginHandler> _logger;
        private readonly IWebConfiguration _config;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public LoginHandler(ILogger<LoginHandler> logger, IWebConfiguration config, IOrganisationQueryRepository organisationQueryRepository, IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _logger = logger;
            _config = config;
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var response =new LoginResponse();
            if (UserDoesNotHaveAcceptableRole(request.Roles))
            {
                _logger.LogInformation("Invalid Role");
                response.Result = LoginResult.InvalidRole;
                return response;
            }

            _logger.LogInformation("Role is good");

            _logger.LogInformation($"Getting Org with ukprn: {request.UkPrn}");
            var organisation = await _organisationQueryRepository.GetByUkPrn(request.UkPrn);

            if (organisation == null)
            {
                _logger.LogInformation($"Org not registered");
                response.Result = LoginResult.NotRegistered;
                return response;
            }

            if (organisation.Status == OrganisationStatus.Deleted)
            {
                _logger.LogInformation($"Org found, but Deleted");
                response.Result = LoginResult.NotRegistered;
                return response;
            }

            _logger.LogInformation($"Got Org with ukprn: {request.UkPrn}, Id: {organisation.EndPointAssessorOrganisationId}");

            var contact = await GetContact(request.Username, request.Email, request.DisplayName);
            if (contact == null)
            {
                await CreateNewContact(request.Email, organisation, request.DisplayName, request.Username);
            }
            response.Result = LoginResult.Valid;
            response.OrganisationName = organisation.EndPointAssessorName;
            return response;
        }

        private bool UserDoesNotHaveAcceptableRole(List<string> roles)
        {
            return !roles.Contains(_config.Authentication.Role);
        }

        private async Task<ContactResponse> GetContact(string username, string email, string displayName)
        {
            _logger.LogInformation($"Getting Contact with username: {username}");
            var contact = await _contactQueryRepository.GetContact(username);
            if (contact == null)
            {
                return null;
            }
            _logger.LogInformation($"Got Existing Contact");
            await CheckStoredUserDetailsForUpdate(contact.Username, email, displayName, contact);
            return contact;
        }

        private async Task CheckStoredUserDetailsForUpdate(string username, string email, string displayName, ContactResponse contactResponse)
        {
            if (contactResponse.Email != email || contactResponse.DisplayName != displayName)
            {
                _logger.LogInformation($"Existing contact has updated details.  Updating");

                await _mediator.Send(new UpdateContactRequest()
                {
                    Email = email,
                    DisplayName = displayName,
                    UserName = username
                });
            }
        }

        private async Task CreateNewContact(string email, Organisation organisation, string displayName,
            string username)
        {
            _logger.LogInformation($"Creating new contact.  Email: {email}, DisplayName: {displayName}, Username: {username}, EndPointAssessorOrganisationId: {organisation.EndPointAssessorOrganisationId}");

            var contact = await _mediator.Send(new CreateContactRequest(){
                Email = email,
                DisplayName = displayName,
                Username = username,
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
            });

            _logger.LogInformation($"New contact created");

            await SetNewOrganisationPrimaryContact(organisation, contact);
        }

        private async Task SetNewOrganisationPrimaryContact(Organisation organisation, ContactResponse contactResponse)
        {
            if (organisation.Status == OrganisationStatus.New)
            {
                _logger.LogInformation($"Org status is New. Setting Org {organisation.EndPointAssessorUkprn} with primary contact of {contactResponse.Username}");

                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    PrimaryContact = contactResponse.Username
                });
            }
        }
    }
}