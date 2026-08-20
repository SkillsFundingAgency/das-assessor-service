using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class FrameworkCertificateSearchResult
    {
        public long Uln { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string CertificateFamilyName { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string CourseLevel { get; set; }

        public DateTime? DateAwarded { get; set; }

        public string ProviderName { get; set; }

        public string Ukprn { get; set; }
    }
}
