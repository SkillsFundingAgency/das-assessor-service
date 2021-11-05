using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAllContactsIncludePrivilegesRequest: IRequest<List<ContactIncludePrivilegesResponse>>
    {
        // @ToDo: .Net Core 3.1 upgrade temporary workaround - adding default constructor to get around deserialization exception
        public GetAllContactsIncludePrivilegesRequest() { }

        public GetAllContactsIncludePrivilegesRequest(string endPointAssessorOrganisationId, bool? withUser = null)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            WithUser = withUser;
        }

        public string EndPointAssessorOrganisationId { get; set; } // @ToDo: .Net Core 3.1 upgrade temporary workaround - adding setter to get around deserialization exception
        public bool? WithUser { get; set; } // @ToDo: .Net Core 3.1 upgrade temporary workaround - adding setter to get around deserialization exception
    }
}
