using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class ReportDetails
    {
        public string Name { get; set; }
        public List<WorksheetDetails> Worksheets { get; set; }
    }
}
