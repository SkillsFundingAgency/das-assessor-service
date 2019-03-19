namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using System.Collections.Generic;

    public class OrganisationSearchResultsViewModel
    { 
        public string Title { get; set; }
        public string SearchTerm { get; set; }
        public IEnumerable<Organisation> SearchResults { get; set; }
        public int TotalCount { get; set; }
    }
}
