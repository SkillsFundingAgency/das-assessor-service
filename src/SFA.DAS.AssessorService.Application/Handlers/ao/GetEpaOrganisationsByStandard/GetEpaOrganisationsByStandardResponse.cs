using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard
{
    public class GetEpaOrganisationsByStandardResponse
    {
        public List<EpaOrganisation> EpaOrganisations { get; set; }
    }
}