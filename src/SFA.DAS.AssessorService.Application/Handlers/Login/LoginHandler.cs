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
        private readonly IMediator _mediator;

        public LoginHandler(ILogger<LoginHandler> logger, IWebConfiguration config, 
            IOrganisationQueryRepository organisationQueryRepository, 
            IContactQueryRepository contactQueryRepository, IMediator mediator)
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

            if (contact.OrganisationId == null)
            {
                var userStatus = await GetUserStatus(null, request.SignInId);
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
                case ContactStatus.Applying:
                    response.Result = LoginResult.Applying;
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
        
    }
}