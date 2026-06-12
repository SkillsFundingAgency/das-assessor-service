namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OppFinderApprovedSearchResult : OppFinderSearchResult
    {
        public int ActiveApprentices { get; set; }

        public int ActiveEPAOs { get; set; }

        public string Versions { get; set; }
    }

    public enum OppFinderApprovedSearchSortColumn
    {
        StandardName,
        ActiveApprentices,
        ActiveEPAOs
    }
}
