namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OppFinderApprovedStandardDetailsRegionResult
  {
    public string Region { get; set; }
    public string[] EndPointAssessorsNames { get; set; }
    public int EndPointAssessors { get; set; }
    public int ActiveApprentices { get; set; }
    public int CompletedAssessments { get; set; }
  }
}
