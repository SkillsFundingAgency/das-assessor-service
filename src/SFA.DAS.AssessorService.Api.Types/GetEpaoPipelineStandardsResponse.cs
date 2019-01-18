using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types
{
    public class GetEpaoPipelineStandardsResponse
    {
        public string StandardName { get; set; }
        public string TrainingProvider { get; set; }
        public int Pipeline { get; set; }
        public string EstimatedDate { get; set; }
    }
}
