using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EPAOPipeline
    {
        public int StdCode { get; set; }
        public int Pipeline { get; set; }
        public DateTime EstimateDate { get; set; }
        public int TotalRows { get; set; }
    }
}
