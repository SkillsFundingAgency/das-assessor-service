namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    using System.Collections.Generic;

    public class OrganisationSearchResults
    {
        public List<Organisation> SearchResults { get; set; }

        public int TotalCount { get; set; }
    }
}
