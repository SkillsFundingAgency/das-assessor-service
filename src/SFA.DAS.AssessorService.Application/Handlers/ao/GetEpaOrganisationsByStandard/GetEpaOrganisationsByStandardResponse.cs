using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard
{
    public class GetEpaOrganisationsByStandardResponse
    {
        public IEnumerable<Organisation> EpaOrganisations { get; set; }
    }
}