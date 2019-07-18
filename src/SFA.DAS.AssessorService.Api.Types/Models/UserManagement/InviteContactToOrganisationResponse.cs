using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.UserManagement
{
    public class InviteContactToOrganisationResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Guid ContactId { get; set; }
    }
}