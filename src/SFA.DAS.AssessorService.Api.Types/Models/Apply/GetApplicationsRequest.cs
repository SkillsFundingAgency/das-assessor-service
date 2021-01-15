using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetApplicationsRequest : IRequest<List<ApplicationResponse>>
    {
        public Guid UserId { get; }

        public string ApplicationType { get; }

        public GetApplicationsRequest(Guid userId, string applicationType)
        {
            UserId = userId;
            ApplicationType = applicationType;
        }
    }
}
