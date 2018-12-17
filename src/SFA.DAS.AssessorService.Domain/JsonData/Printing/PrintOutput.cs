using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Printing
{
    public class PrintOutput
    {
        public BatchDetails Batch { get; set; }
        public List<PrintData> PrintData { get; set; }
    }
}
