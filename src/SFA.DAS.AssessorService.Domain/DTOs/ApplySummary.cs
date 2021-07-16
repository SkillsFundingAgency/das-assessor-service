using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class ApplySummary
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ReviewStatus { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public ApplyData ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmail { get; set; }
        public string StandardApplicationType { get; set; }
    }
}
