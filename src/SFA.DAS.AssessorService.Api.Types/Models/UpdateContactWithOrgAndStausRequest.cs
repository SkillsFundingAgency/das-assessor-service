﻿using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateContactWithOrgAndStausRequest: IRequest<Unit>
    {
        public UpdateContactWithOrgAndStausRequest(string contactId, string orgId, string epaOrgId, string status)
        {
            ContactId = contactId;
            OrgId = orgId;
            EpaOrgId = epaOrgId;
            Status = status;
        }

        public string ContactId { get; }
        public string OrgId { get; }
        public string EpaOrgId { get; }
        public string Status { get; }

    }
}
