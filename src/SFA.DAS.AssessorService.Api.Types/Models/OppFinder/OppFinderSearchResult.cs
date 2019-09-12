namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OppFinderSearchResult
    {
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
    }

    public enum OppFinderSearchSortColumn
    {
        StandardName
    }
}
