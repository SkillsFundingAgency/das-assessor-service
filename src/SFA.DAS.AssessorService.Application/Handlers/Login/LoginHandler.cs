using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly ILogger<LoginHandler> _logger;
        private readonly IWebConfiguration _config;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly IContactRepository _contactRepository;

        public LoginHandler(ILogger<LoginHandler> logger, IWebConfiguration config, 
            IOrganisationQueryRepository organisationQueryRepository, 
            IContactQueryRepository contactQueryRepository, IContactRepository contactRepository,
            IRegisterRepository registerRepository)
        {
            _logger = logger;
            _config = config;
            _organisationQueryRepository = organisationQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _contactRepository = contactRepository;
            _registerRepository = registerRepository;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var response =new LoginResponse();

            var contact = await _contactQueryRepository.GetBySignInId(request.SignInId);

            //ON-1926 Check if username is the same as the email if not update the username so that it is
            //Since in aslogin the username is the email address of the 
            var originalUsername = contact.Username;

            if (string.IsNullOrEmpty(originalUsername) ||
                !contact.Username.Equals(contact.Email, StringComparison.OrdinalIgnoreCase))
                await _contactRepository.UpdateUserName(contact.Id,contact.Email);

            if (await UserDoesNotHaveAcceptableRole(contact.Id))
            {
                _logger.LogInformation("Invalid Role");
                _logger.LogInformation(LoggingConstants.SignInIncorrectRole);
                response.Result = LoginResult.InvalidRole;
                return response;
            }
            
            _logger.LogInformation("Role is good");

            if (contact.OrganisationId == null)
            {
                var userStatus = contact.Status;// await GetUserStatus(null, request.SignInId);
                if (userStatus != ContactStatus.Applying)
                {
                    response.Result = LoginResult.NotRegistered;
                    return response;
                }
                else
                {
                    response.Result = LoginResult.Applying;
                    return response;
                }
            }

            var organisation = await _organisationQueryRepository.Get(contact.OrganisationId.Value);


            if (organisation == null)
            {
                _logger.LogInformation($"Org not registered");
                _logger.LogInformation(LoggingConstants.SignInNotAnEpao);
                response.Result = LoginResult.NotRegistered;
                return response;
            }

            //ON-1926 If there was an organisation associated with the current contact then if the primary contact in 
            //that organisation matches the orginal username then make sure it is updated to reflect the latest username.
            if (!string.IsNullOrEmpty(originalUsername))
                await _registerRepository.UpdateEpaOrganisationPrimaryContact(contact.Id, originalUsername);

            response.EndPointAssessorName = organisation.EndPointAssessorName;

            response.EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId;

            _logger.LogInformation($"Got Org with ukprn: {organisation.EndPointAssessorUkprn}, Id: {organisation.EndPointAssessorOrganisationId}, Status: {organisation.Status}");

            if (organisation.Status == OrganisationStatus.Deleted)
            {
                _logger.LogInformation(LoggingConstants.SignInEpaoDeleted);
                response.Result = LoginResult.NotRegistered;
                return response;
            }
            else if (organisation.Status == OrganisationStatus.New)
            {
                _logger.LogInformation(LoggingConstants.SignInEpaoNew);

                // Only show true status if contact is marked as being Live
                response.Result = (contact.Status is ContactStatus.Live) ? LoginResult.NotActivated : LoginResult.InvitePending;
                return response;
            }
            else
            {
                _logger.LogInformation(LoggingConstants.SignInSuccessful);

                var status = contact.Status; //await GetUserStatus(organisation.EndPointAssessorOrganisationId, request.SignInId);
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
                    case ContactStatus.Applying:
                        response.Result = LoginResult.Applying;
                        break;
                    default:
                        response.Result = LoginResult.NotRegistered;
                        break;
                }
            }

            return response;
        }

        private async Task<bool> UserDoesNotHaveAcceptableRole(Guid contactId)
        {
            return false;
//            var roles = await _contactQueryRepository.GetRolesFor(contactId);
//            return roles.All(r => r.RoleName != "SuperUser");
//                
//            //TODO: This needs to look up the user by the id and check they are in the appropriate role.
//            //return !roles.Contains(_config.Authentication.Role);
        }

//        private async Task<string> GetUserStatus(string endPointAssessorOrganisationId, Guid signInId)
//        {
//            return await _contactQueryRepository.GetContactStatus(endPointAssessorOrganisationId, signInId);
//        }
        
    }
}