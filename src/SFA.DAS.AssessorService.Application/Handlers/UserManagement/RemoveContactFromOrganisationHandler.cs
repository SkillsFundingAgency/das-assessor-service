using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Handlers.UserManagement
{
    public class RemoveContactFromOrganisationHandler : IRequestHandler<RemoveContactFromOrganisationRequest, RemoveContactFromOrganisationResponse>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IContactRepository _contactRepository;

        public RemoveContactFromOrganisationHandler(IContactQueryRepository contactQueryRepository, IContactRepository contactRepository) 
        {
            _contactQueryRepository = contactQueryRepository;
            _contactRepository = contactRepository;
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
            await _contactRepository.RemoveAllPrivileges(request.ContactId);
            await _contactRepository.CreateContactLog(request.RequestingUserId, request.ContactId, ContactLogType.UserRemoved, 
                null);
            
            
            await LogContactRemovedChanges(request);

            return new RemoveContactFromOrganisationResponse()
            {
                Success = true,
                SelfRemoved = request.ContactId == request.RequestingUserId
            };
        }

        private async Task LogContactRemovedChanges(RemoveContactFromOrganisationRequest request)
        {
            await _contactRepository.CreateContactLog(
                request.RequestingUserId,
                request.ContactId,
                request.RequestingUserId.Equals(Guid.Empty)
                    ? ContactLogType.UserRemovedByStaff
                    : ContactLogType.UserRemoved,
                 null);
        }
    }
}