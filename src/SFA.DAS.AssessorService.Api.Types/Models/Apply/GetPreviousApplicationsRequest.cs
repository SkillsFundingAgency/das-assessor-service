using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetPreviousApplicationsRequest : IRequest<List<ApplicationResponse>>
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
