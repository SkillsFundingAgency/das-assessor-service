using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard
{
    public class GetEpaOrganisationsByStandardResponse
    {
        public List<OrganisationResponse> EpaOrganisations { get; set; }
    }
}