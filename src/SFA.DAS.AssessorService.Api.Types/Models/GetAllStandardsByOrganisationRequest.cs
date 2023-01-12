using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAllStandardsByOrganisationRequest : IRequest<List<OrganisationStandardSummary>>
    {
        public string OrganisationId;
    }
}
