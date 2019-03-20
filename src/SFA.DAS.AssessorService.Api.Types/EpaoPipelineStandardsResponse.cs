using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class EpaoPipelineStandardsResponse
    {
        public string StandardName { get; set; }
        public int StandardCode{ get; set; }
        public int Pipeline { get; set; }
        public string EstimatedDate { get; set; }
    }
}
