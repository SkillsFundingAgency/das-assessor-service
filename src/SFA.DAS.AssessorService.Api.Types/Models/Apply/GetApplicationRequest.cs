using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetApplicationRequest : IRequest<ApplicationResponse>
    {
        public Guid ApplicationId { get; }
        public Guid? UserId { get; }

        public GetApplicationRequest(Guid applicationId, Guid? userId = null)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
