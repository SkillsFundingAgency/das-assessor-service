using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.JsonData.Printing
{
    public class PrintOutput
    {
        public BatchDetails Batch { get; set; }
        public List<PrintData> PrintData { get; set; }
    }
}
