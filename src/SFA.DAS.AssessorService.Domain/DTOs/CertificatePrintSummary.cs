using System;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class CertificatePrintSummary : CertificatePrintSummaryBase
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorOrganisationName { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public bool CoronationEmblem { get; set; }
        public string ContactOrganisation { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string Department { get; set; }
    }
}
