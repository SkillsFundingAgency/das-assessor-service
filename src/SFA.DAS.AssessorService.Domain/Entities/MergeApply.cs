using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class MergeApply : BaseEntity
    {
        public static MergeApply CreateFrom(ApplyEF application)
        {
            var mergeApply = new MergeApply()
            {
                ApplyId = application.Id,
                StandardApplicationType = application.StandardApplicationType,
                ApplicationId = application.ApplicationId,
                OrganisationId = application.OrganisationId,
                ApplicationStatus = application.ApplicationStatus,
                FinancialReviewStatus = application.FinancialReviewStatus,
                ReviewStatus = application.ReviewStatus,
                FinancialGrade = application.FinancialGrade,
                StandardCode = application.StandardCode,
                ApplyData = application.ApplyData,
                CreatedBy = application.CreatedBy,
                UpdatedBy = application.UpdatedBy,
                DeletedBy = application.DeletedBy,
                StandardReference = application.StandardReference,
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt,
                DeletedAt = application.DeletedAt
            };
            return mergeApply;
        }

        // Merge
        public int Id { get; set; }
        public int MergeOrganisationId { get; set; }
        public virtual MergeOrganisation MergeOrganisation { get; set; }
        public string Replicates { get; set; }

        // Apply
        public Guid ApplyId { get; set; }
        public string StandardApplicationType { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialGrade { get; set; }
        public int? StandardCode { get; set; }
        public string ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string StandardReference { get; set; }
    }
}
