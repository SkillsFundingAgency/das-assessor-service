using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAssessmentsResponse
    {
        public DateTime? EarliestAssessment { get; }
        public int EndpointAssessmentCount { get; }

        public GetAssessmentsResponse(DateTime? earliestAssessment, int endpointAssessmentCount)
        {
            EarliestAssessment = earliestAssessment;
            EndpointAssessmentCount = endpointAssessmentCount;
        }
    }
}
