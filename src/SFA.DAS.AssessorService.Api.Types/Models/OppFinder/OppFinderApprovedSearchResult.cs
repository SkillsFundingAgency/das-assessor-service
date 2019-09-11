namespace SFA.DAS.AssessorService.Api.Types.Models.OppFinder
{
    public class OppFinderApprovedSearchResult : OppFinderSearchResult
    {
        public int ActiveApprentices { get; set; }

        public int RegisteredEPAOs { get; set; }
    }

    public enum OppFinderApprovedSearchSortColumn
    {
        Name,
        ActiveApprentices,
        RegisteredEPAOs
    }
}
