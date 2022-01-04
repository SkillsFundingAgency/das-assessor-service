using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetPreviousApplicationsRequest : IRequest<ApplicationResponse>
    {
        public Guid OrgId { get; }
        public string StandardReference { get; }

        public GetPreviousApplicationsRequest(Guid orgId, string standardReference)
        {
            OrgId = orgId;
            StandardReference = standardReference;
        }
    }
}
