namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderNonApprovedDetailsViewModel
    {
        public NonApprovedType NonApprovedType { get; set; }
        public int PageIndex { get; set; }
        public string Title { get; set; }
        public string OverviewOfRole { get; set; }
        public string StandardLevel { get; set; }
        public string StandardReference { get; set; }
        public string Sector { get; set; }
        public int TypicalDuration { get; set; }
        public string[] Trailblazer { get; set; }
        public string StandardPageUrl { get; set; }
    }

    public enum NonApprovedType
    {
        InDevelopment,
        Proposed
    }
}
