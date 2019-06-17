using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class NotifyUserManagementUsersRequest : IRequest
    {
        public NotifyUserManagementUsersRequest(string displayName, string endPointAssessorOrganisationId, string serviceLink)
        {
            DisplayName = displayName;
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            ServiceLink = serviceLink;
        }

        public string DisplayName { get; }
        public string EndPointAssessorOrganisationId { get; }
        public string ServiceLink { get; }
    }
}
