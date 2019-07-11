using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsWithPrivilegesRequest: IRequest<List<ContactsWithPrivilegesResponse>>
    {
        public GetContactsWithPrivilegesRequest(Guid organisationId)
        {
            OrganisationId = organisationId;
        }

        public Guid OrganisationId { get;}
    }
}
