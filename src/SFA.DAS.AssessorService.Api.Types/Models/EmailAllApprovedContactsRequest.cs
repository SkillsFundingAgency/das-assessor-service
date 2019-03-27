using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class EmailAllApprovedContactsRequest
    {
        public EmailAllApprovedContactsRequest(string displayName, string organisationReferenceId, string serviceLibnk)
        {
            DisplayName = displayName;
            OrganisationReferenceId = organisationReferenceId;
            ServiceLink = serviceLibnk;
        }

        public string DisplayName { get; }
        public string OrganisationReferenceId { get; }
        public string ServiceLink { get; }
    }
}
