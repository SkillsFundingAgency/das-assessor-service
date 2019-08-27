using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ContactInvitation
    {
        public Guid Id { get; set; }
        public DateTime InvitationDate { get; set; }
        public Guid InvitorContactId { get; set; }
        public Guid InviteeContactId { get; set; }
        public bool IsAccountCreated { get; set; }
        public DateTime AccountCreatedDate { get; set; }
    }
}