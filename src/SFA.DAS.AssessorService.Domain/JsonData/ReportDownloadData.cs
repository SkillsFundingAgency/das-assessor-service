using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.JsonData
{
    public class ReportDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<WorksheetDetails> Worksheets { get; set; }
    }
}
