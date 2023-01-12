using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAllContactsRequest : IRequest<List<ContactResponse>>
    {
        public GetAllContactsRequest(string endPointAssessorOrganisationId, bool? withUser = null)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
            WithUser = withUser;
        }

        public string EndPointAssessorOrganisationId { get; }
        public bool? WithUser { get; }
    }
}
