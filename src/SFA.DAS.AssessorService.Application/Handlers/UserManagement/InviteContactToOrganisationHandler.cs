using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class InviteContactToOrganisationHandler : IRequestHandler<InviteContactToOrganisationRequest, InviteContactToOrganisationResponse>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IContactRepository _contactRepository;

        public InviteContactToOrganisationHandler(IContactQueryRepository contactQueryRepository, IContactRepository contactRepository)
        {
            _contactQueryRepository = contactQueryRepository;
            _contactRepository = contactRepository;
        }
        
        
        public async Task<InviteContactToOrganisationResponse> Handle(InviteContactToOrganisationRequest request, CancellationToken cancellationToken)
        {
            var existingContact = await _contactQueryRepository.GetContactFromEmailAddress(request.Email);
            if (existingContact != null)
            {
                if (existingContact.Status == ContactStatus.Live)
                {
                    return new InviteContactToOrganisationResponse()
                    {
                        Success = false,
                        ErrorMessage = existingContact.OrganisationId == request.OrganisationId
                            ? "This email address is already registered against your organisation. You must use a unique email address."
                            : "This email address is already registered against another organisation. You must use a unique email address."

                    };
                }
                // If it does exist, if it's still new, update it with this org.
            }
            else
            {
                // If doesn't exist, create a new contact for this organisation.  Status new.
                
            }
            
            // Send invite to loginApi.
            return new InviteContactToOrganisationResponse();
        }
    }
}