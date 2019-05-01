using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsWithPrivilagesRequest: IRequest<List<ContactsWithPrivilegesResponse>>
    {
        public GetContactsWithPrivilagesRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }

        public string EndPointAssessorOrganisationId { get;}
    }
}
