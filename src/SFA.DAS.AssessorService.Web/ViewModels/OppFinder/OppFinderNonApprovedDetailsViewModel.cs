namespace SFA.DAS.AssessorService.Web.ViewModels.OppFinder
{
    public class OppFinderNonApprovedDetailsViewModel : OppFinderDetailsViewModel
    {
        public NonApprovedType NonApprovedType { get; set; }
    }

    public enum NonApprovedType
    {
        InDevelopment,
        Proposed
    }
}
