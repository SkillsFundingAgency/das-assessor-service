using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class NotifyInvitationAcceptedRequest : IRequest
    {
        public Guid InvitingContactId { get; }
        public Guid AcceptedContactId { get; }

        public NotifyInvitationAcceptedRequest(Guid invitingContactId, Guid acceptedContactId)
        {
            InvitingContactId = invitingContactId;
            AcceptedContactId = acceptedContactId;
        }
    }
}
