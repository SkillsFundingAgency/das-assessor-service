using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class OrganisationSearchResults
    {
        public List<Organisation> SearchResults { get; set; }

        public int TotalCount { get; set; }
    }
}
