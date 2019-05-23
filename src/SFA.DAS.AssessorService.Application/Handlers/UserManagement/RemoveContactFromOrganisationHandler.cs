using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class RemoveContactFromOrganisationHandler : IRequestHandler<RemoveContactFromOrganisationRequest, RemoveContactFromOrganisationResponse>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IContactApplyClient _applyContactsApiClient;

        public RemoveContactFromOrganisationHandler(IContactQueryRepository contactQueryRepository, IContactRepository contactRepository, IContactApplyClient applyContactsApiClient) 
        {
            _contactQueryRepository = contactQueryRepository;
            _contactRepository = contactRepository;
            _applyContactsApiClient = applyContactsApiClient;
        }

        public async Task<RemoveContactFromOrganisationResponse> Handle(RemoveContactFromOrganisationRequest request, CancellationToken cancellationToken)
        {
            var currentPrivileges = (await _contactQueryRepository.GetPrivilegesFor(request.ContactId));

            foreach (var currentPrivilege in currentPrivileges.Where(p => p.Privilege.MustBeAtLeastOneUserAssigned))
            {
                if (await _contactRepository.IsOnlyContactWithPrivilege(request.ContactId, currentPrivilege.PrivilegeId))
                {
                    return new RemoveContactFromOrganisationResponse()
                    {
                        Success = false, 
                        ErrorMessage = $"Before you remove this user, you must assign '{currentPrivilege.Privilege.UserPrivilege}' to another user."
                    };
                }
            }

            await _contactRepository.RemoveContactFromOrganisation(request.ContactId);

            await _applyContactsApiClient.RemoveContactFromOrganisation(request.ContactId);
            
            await _contactRepository.CreateContactLog(request.RequestingUserId, request.ContactId, ContactLogType.UserRemoved, 
                null);
            
            return new RemoveContactFromOrganisationResponse()
            {
                Success = true,
                SelfRemoved = request.ContactId == request.RequestingUserId
            };
        }
    }
}