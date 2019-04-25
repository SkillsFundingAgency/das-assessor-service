using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.JsonData.Printing
{
    public class PrintOutput
    {
        public BatchData Batch { get; set; }
        public List<PrintData> PrintData { get; set; }
    }
}
