using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class InviteContactToOrganisationRequest : IRequest<InviteContactToOrganisationResponse>
    {
        public Guid InvitedByContactId { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Email { get; set; }
        public Guid OrganisationId { get; set; }
        
        public Guid[] Privileges { get; set; }
    }
}