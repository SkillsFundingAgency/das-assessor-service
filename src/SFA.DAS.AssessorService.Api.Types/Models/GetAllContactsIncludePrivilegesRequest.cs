using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAllContactsIncludePrivilegesRequest: IRequest<List<ContactIncludePrivilegesResponse>>
    {
        public GetAllContactsIncludePrivilegesRequest(string endPointAssessorOrganisationId, bool? withUser = null)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            WithUser = withUser;
        }

        public string EndPointAssessorOrganisationId { get;}
        public bool? WithUser { get; }
    }
}
