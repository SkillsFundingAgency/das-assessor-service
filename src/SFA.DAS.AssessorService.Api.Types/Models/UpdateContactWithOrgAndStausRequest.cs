using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateContactWithOrgAndStausRequest: IRequest
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
