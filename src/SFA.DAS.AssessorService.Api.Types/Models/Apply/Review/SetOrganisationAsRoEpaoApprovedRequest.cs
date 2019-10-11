using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class SetOrganisationAsRoEpaoApprovedRequest : IRequest
    {
        public Guid ApplicationId { get; }
        public Guid OrganisationId { get; }

        public SetOrganisationAsRoEpaoApprovedRequest(Guid applicationId, Guid organisationId)
        {
            ApplicationId = applicationId;
            OrganisationId = organisationId;
        }
    }
}
