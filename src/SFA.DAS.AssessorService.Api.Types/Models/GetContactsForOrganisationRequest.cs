using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactsForOrganisationRequest : IRequest<List<ContactResponse>>
    {
        public GetContactsForOrganisationRequest(string endPointAssessorOrganisationId)
        {
            EndPointAssessorOrganisationId = endPointAssessorOrganisationId;
        }

        public string EndPointAssessorOrganisationId { get; }
    }

}
