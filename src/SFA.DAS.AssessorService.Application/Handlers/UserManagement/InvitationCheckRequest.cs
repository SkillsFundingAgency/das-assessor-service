using System;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class InvitationCheckRequest : IRequest<Unit>
    {
        public Guid ContactId { get; }

        public InvitationCheckRequest(Guid contactId)
        {
            ContactId = contactId;
        }
    }
}