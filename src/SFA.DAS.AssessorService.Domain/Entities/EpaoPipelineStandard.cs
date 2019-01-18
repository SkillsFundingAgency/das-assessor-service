using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EpaoPipelineStandard
    {
        public string Title { get; set; }
        public int Pipeline { get; set; }
        public DateTime EstimateDate { get; set; }
        public int TotalRows { get; set; }
    }
}
