using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetAllWithdrawnApplicationsForStandardRequest : IRequest<List<ApplicationResponse>>
    {
        public Guid OrgId { get; }
        public int? StandardCode { get; }

        public GetAllWithdrawnApplicationsForStandardRequest(Guid orgId, int? standardCode)
        {
            OrgId = orgId;
            StandardCode = standardCode;
        }
    }
}
