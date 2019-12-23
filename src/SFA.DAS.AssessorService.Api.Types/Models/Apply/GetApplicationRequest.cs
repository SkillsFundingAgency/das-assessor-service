using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetApplicationRequest : IRequest<ApplicationResponse>
    {
        public Guid ApplicationId { get; }

        public GetApplicationRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
