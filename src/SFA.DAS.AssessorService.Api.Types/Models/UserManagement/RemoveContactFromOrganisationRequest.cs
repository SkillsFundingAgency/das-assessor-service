using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class RemoveContactFromOrganisationRequest : IRequest<RemoveContactFromOrganisationResponse>
    {
        public Guid RequestingUserId { get; }
        public Guid ContactId { get; }

        public RemoveContactFromOrganisationRequest(Guid requestingUserId, Guid contactId)
        {
            RequestingUserId = requestingUserId;
            ContactId = contactId;
        }
    }
}