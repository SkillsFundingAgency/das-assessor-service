using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class SetContactPrivilegesHandler : IRequestHandler<SetContactPrivilegesRequest, SetContactPrivilegesResponse>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public SetContactPrivilegesHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }
        
        public async Task<SetContactPrivilegesResponse> Handle(SetContactPrivilegesRequest request, CancellationToken cancellationToken)
        {
            // Work out which privileges are being removed....
            //     Get current contact privileges
            //     For those not in request.privilegeids, that have MustBeAtLeastOneUserAssigned true,
            //     Check that this contact is NOT the only contact in the org to have that privilege.

            var currentPrivileges = (await _contactQueryRepository.GetPrivilegesFor(request.ContactId));

            var privilegesBeingRemoved = currentPrivileges.Where(cp => !request.PrivilegeIds.Contains(cp.PrivilegeId));
            
            
            var privilegesBeingRemovedThatMustBelongToSomeone = privilegesBeingRemoved.Where(p => p.Privilege.MustBeAtLeastOneUserAssigned).ToList();

            foreach (var currentPrivilege in privilegesBeingRemovedThatMustBelongToSomeone)
            {
                if (await _contactRepository.IsOnlyContactWithPrivilege(request.ContactId, currentPrivilege.PrivilegeId))
                {
                    return new SetContactPrivilegesResponse()
                    {
                        Success = false, 
                        ErrorMessage = $"{currentPrivilege.Privilege.UserPrivilege} cannot be removed as user is the last with this privilege."
                    };
                }
            }
            
            await _contactRepository.RemoveAllPrivileges(request.ContactId);

            foreach (var privilegeId in request.PrivilegeIds)
            {
                await _contactRepository.AddPrivilege(request.ContactId, privilegeId);
            }

            return new SetContactPrivilegesResponse() {Success = true};
        }
    }
}