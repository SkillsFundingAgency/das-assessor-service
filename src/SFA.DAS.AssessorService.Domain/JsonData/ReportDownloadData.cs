using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.JsonData
{
    public class ReportDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<WorksheetDetails> Worksheets { get; set; }
    }
}
