using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class FrameworkSearchResult
    {
        public Guid Id { get; set; }
        public string FrameworkName { get; set; }
        public string ApprenticeshipLevel { get; set; }
        public int CertificationYear { get; set; }
    }
}