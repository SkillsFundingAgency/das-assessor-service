using System;

namespace SFA.DAS.AssessorService.Domain.DTOs.Certificate
{
    public class AssessmentsResult
    {
        public DateTime? EarliestAssessment { get; set; }
        public int EndpointAssessmentCount { get; set; }
    }
}
