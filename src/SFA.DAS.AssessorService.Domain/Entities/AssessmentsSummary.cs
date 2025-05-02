using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class AssessmentsSummary
	{
		public Guid Id { get; set; }		
		public long Ukprn { get; set; }
        public string IfateReferenceNumber { get; set; }
        public DateTime EarliestAssessment { get; set; }
        public int EndpointAssessmentCount { get; set; }
		public DateTime UpdatedA { get; set; }
	}
}
