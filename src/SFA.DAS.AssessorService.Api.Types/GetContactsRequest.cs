using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class GetContactsRequest: IRequest<List<ContactsWithRolesResponse>>
    {
        public GetContactsRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }

        public string EndPointAssessorOrganisationId { get;}
    }
}
