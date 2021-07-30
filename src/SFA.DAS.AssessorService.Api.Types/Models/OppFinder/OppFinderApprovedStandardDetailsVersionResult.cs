using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
  public class OppFinderApprovedStandardDetailsVersionResult
    {
    public string Version { get; set; }
    public int EndPointAssessors { get; set; }
    public int ActiveApprentices { get; set; }
    public int CompletedAssessments { get; set; }
  }
}
