using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsRequest: IRequest<List<ContactsWithPrivilegesResponse>>
    {
        public GetContactsRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }

        public string EndPointAssessorOrganisationId { get;}
    }
}
