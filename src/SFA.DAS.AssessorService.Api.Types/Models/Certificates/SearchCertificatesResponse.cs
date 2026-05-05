using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class SearchCertificatesResponse
    {
        public long Uln { get; set; }
        public string CertificateType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseLevel { get; set; }
        public DateTime? DateAwarded { get; set; }
        public string ProviderName { get; set; }
        public string Ukprn { get; set; }
    }
}
