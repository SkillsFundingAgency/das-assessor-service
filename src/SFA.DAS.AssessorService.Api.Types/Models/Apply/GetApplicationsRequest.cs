using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class GetApplicationsRequest : IRequest<List<ApplicationResponse>>
    {
        public Guid UserId { get; }

        public bool CreatedBy { get; }

        public GetApplicationsRequest(Guid userId, bool createdBy)
        {
            UserId = userId;
            CreatedBy = createdBy;
        }
    }
}
