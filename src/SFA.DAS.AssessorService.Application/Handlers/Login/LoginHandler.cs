using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
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

            var contact = await _contactQueryRepository.GetBySignInId(request.SignInId);
            
            if (await UserDoesNotHaveAcceptableRole(contact.Id))
            {
                _logger.LogInformation("Invalid Role");
                _logger.LogInformation(LoggingConstants.SignInIncorrectRole);
                response.Result = LoginResult.InvalidRole;
                return response;
            }
            
            _logger.LogInformation("Role is good");
            
            var organisation = await _organisationQueryRepository.Get(contact.OrganisationId.Value);

            if (organisation == null)
            {
                _logger.LogInformation($"Org not registered");
                _logger.LogInformation(LoggingConstants.SignInNotAnEpao);
                response.Result = LoginResult.NotRegistered;
                return response;
            }

            if (organisation.Status == OrganisationStatus.Deleted)
            {
                _logger.LogInformation($"Org found, but Deleted");
                _logger.LogInformation(LoggingConstants.SignInEpaoDeleted);
                response.Result = LoginResult.NotRegistered;
                return response;
            }

            _logger.LogInformation($"Got Org with ukprn: {organisation.EndPointAssessorUkprn}, Id: {organisation.EndPointAssessorOrganisationId}");

            _logger.LogInformation(LoggingConstants.SignInSuccessful);

            var status = await GetUserStatus(organisation.EndPointAssessorOrganisationId, request.SignInId);
            switch (status)
            {
                case ContactStatus.Live:
                    response.Result = LoginResult.Valid;
                    break;
                case ContactStatus.InvitePending:
                    response.Result = LoginResult.InvitePending;
                    break;
                case ContactStatus.Inactive:
                    response.Result = LoginResult.Rejected;
                    break;
                default:
                    response.Result = LoginResult.NotRegistered;
                    break;
            }


            response.OrganisationName = organisation.EndPointAssessorName;
            return response;
        }

        private async Task<bool> UserDoesNotHaveAcceptableRole(Guid contactId)
        {
            var roles = await _contactQueryRepository.GetRolesFor(contactId);
            return roles.All(r => r.RoleName != "SuperUser");
                
            //TODO: This needs to look up the user by the id and check they are in the appropriate role.
            //return !roles.Contains(_config.Authentication.Role);
        }

        private async Task<string> GetUserStatus(string endPointAssessorOrganisationId, Guid signInId)
        {
            return await _contactQueryRepository.GetContactStatus(endPointAssessorOrganisationId, signInId);
        }

        private async Task<Contact> GetContact(string username, string email, string displayName)
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

        private async Task CheckStoredUserDetailsForUpdate(string username, string email, string displayName, Contact contact)
        {
            if (contact.Email != email || contact.DisplayName != displayName)
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

        private async Task SetNewOrganisationPrimaryContact(Organisation organisation, Contact contact)
        {
            if (organisation.Status == OrganisationStatus.New)
            {
                _logger.LogInformation($"Org status is New. Setting Org {organisation.EndPointAssessorUkprn} with primary contact of {contact.Username}");

                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    PrimaryContact = contact.Username,
                    EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                    ApiEnabled = organisation.ApiEnabled,
                    ApiUser = organisation.ApiUser
                });
            }
        }
    }
}