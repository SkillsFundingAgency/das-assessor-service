using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsForOrganisationRequest : IRequest<List<ContactResponse>>
    {
        public GetContactsForOrganisationRequest(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public Guid OrganisationId { get; }
    }
}
