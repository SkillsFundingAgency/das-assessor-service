using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EmailAllApprovedContactsRequest
    {
        public EmailAllApprovedContactsRequest(string displayName, string organisationReferenceId)
        {
            DisplayName = displayName;
            OrganisationReferenceId = organisationReferenceId;
        }

        public string DisplayName { get; }
        public string OrganisationReferenceId { get; }
    }
}
