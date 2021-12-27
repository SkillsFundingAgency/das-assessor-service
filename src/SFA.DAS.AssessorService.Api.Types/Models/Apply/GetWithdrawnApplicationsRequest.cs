using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetWithdrawnApplicationsRequest : IRequest<ApplicationResponse>
    {
        public Guid ApplicationId { get; }
        public int? StandardCode { get; }

        public GetWithdrawnApplicationsRequest(Guid applicationId, int? standardCode)
        {
            ApplicationId = applicationId;
            StandardCode = standardCode;
        }
    }
}
