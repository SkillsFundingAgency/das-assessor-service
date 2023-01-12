using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsWhoCanBePrimaryForOrganisationRequest : IRequest<List<ContactResponse>>
    {
        public GetContactsWhoCanBePrimaryForOrganisationRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }

        public string EndPointAssessorOrganisationId { get; }
    }

}
