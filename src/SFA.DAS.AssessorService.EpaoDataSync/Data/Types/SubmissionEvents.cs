using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoDataSync.Data.Types
{
    public class SubmissionEvents
    {
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public List<SubmissionEvent> Items { get; set; }
    }
}
