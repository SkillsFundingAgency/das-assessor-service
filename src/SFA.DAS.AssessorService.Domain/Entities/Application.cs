using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Application: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ReviewStatus { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public int StandardCode { get; set; }
        public ApplyData ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
    }
}
