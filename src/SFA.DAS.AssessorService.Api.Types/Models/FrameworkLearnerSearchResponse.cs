using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class FrameworkLearnerSearchResponse
    {
        public Guid Id { get; set; }
        public string FrameworkName { get; set; }
        public string ApprenticeshipLevelName { get; set; }
        public string CertificationYear { get; set; }
    }
}